using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [SerializeField]
    CombatPathfinder _pathfinder;
    [SerializeField]
    Tile MovableTile;
    [SerializeField]
    Camera _camera;
    [SerializeField]
    UIView_SumBattle _sumBattleUI;

    public Tilemap _walkable;
    public Tilemap _obstacles;
    public Tilemap _ui;

    public Vector4 _bounds;

    public int XMin => (int)_bounds.x;
    public int XMax => (int)_bounds.y;
    public int YMin => (int)_bounds.z;
    public int YMax => (int)_bounds.w;

    UnitCombatView[] _attackerUnitViews = new UnitCombatView[7];
    UnitCombatView[] _defenderUnitViews = new UnitCombatView[7];
    List<UnitInstance> _turnOrder = new();
    int _currentUnitIndex = 0;
    UnitInstance _currentUnit => _turnOrder[_currentUnitIndex];

    ICombatant _attacker;
    (UnitModel, int)[] _attackerStartingUnits = new (UnitModel, int)[7];
    ICombatant _defender;
    (UnitModel, int)[] _defenderStartingUnits = new (UnitModel, int)[7];

    Vector3Int[] _leftPositions;
    Vector3Int[] _rightPositions;
    UnitAI _ai;

    public void Init(ICombatant attacker, ICombatant defender)
    {
        _attacker = attacker;
        _defender = defender;
        _ai = new UnitAI(this);

        for (int i = 0; i < _attacker.Units.Length; i++)
        {
            UnitInstance unit = _attacker.Units[i];
            if (unit != null)
                _attackerStartingUnits[i] = (unit.Model, unit.Amount);
        }
        for (int i = 0; i < _defender.Units.Length; i++)
        {
            UnitInstance unit = _defender.Units[i];
            if (unit != null)
                _defenderStartingUnits[i] = (unit.Model, unit.Amount);
        }

        _leftPositions = new Vector3Int[]
        {
            new Vector3Int(XMin, YMax, 0),
            new Vector3Int(XMin, YMax -2, 0),
            new Vector3Int(XMin, YMax -4, 0),
            new Vector3Int(XMin + 1, YMax -5, 0),
            new Vector3Int(XMin, YMax - 6, 0),
            new Vector3Int(XMin, YMax - 8, 0),
            new Vector3Int(XMin, YMax - 10, 0),
        };

        _rightPositions = new Vector3Int[]
        {
            new Vector3Int(XMax, YMax, 0),
            new Vector3Int(XMax, YMax -2, 0),
            new Vector3Int(XMax, YMax -4, 0),
            new Vector3Int(XMax, YMax -5, 0),
            new Vector3Int(XMax, YMax - 6, 0),
            new Vector3Int(XMax, YMax - 8, 0),
            new Vector3Int(XMax, YMax - 10, 0),
        };

        InitializeUnits(attacker, _attackerUnitViews, _leftPositions, true);
        InitializeUnits(defender, _defenderUnitViews, _rightPositions, false);

        _turnOrder = attacker.Units.
            Concat(defender.Units).
            Where(u => u != null).
            OrderByDescending(u => u.Model.Speed).
            ToList();

        StartCoroutine(CombatSequenceRoutine());
    }

    private IEnumerator CombatSequenceRoutine()
    {
        while (true)
        {
            if (_currentUnit == null)
            {
                _turnOrder.RemoveAt(_currentUnitIndex);
                if (_currentUnitIndex >= _turnOrder.Count)
                    _currentUnitIndex = 0;
                continue;
            }

            _currentUnit.HasRetaliation = true;
            _currentUnit.Selected = true;
            RemoveAllWalkables();
            ShowUnitWalkables(_currentUnit);

            if (_currentUnit.Controlled)
                yield return StartCoroutine(PlayerUnitTurnRoutine());
            else
                yield return StartCoroutine(UnitAIRoutine());

            if (_currentUnit != null)
                _currentUnit.Selected = false;
            else
                _turnOrder.RemoveAt(_currentUnitIndex);

            _currentUnitIndex = (_currentUnitIndex + 1) % _turnOrder.Count;

            var attackerWon = _defender.Units.All(u => u == null);
            if (_attacker.Units.All(u => u == null) || _defender.Units.All(u => u == null))
            {
                var ui = Instantiate(_sumBattleUI);
                (UnitModel, int)[] attackerCasualties = new (UnitModel, int)[7];
                for (int i = 0; i < _attacker.Units.Length; i++)
                {
                    UnitInstance unit = _attacker.Units[i];
                    if (unit != null)
                        attackerCasualties[i] = (unit.Model, _attackerStartingUnits[i].Item2 - unit.Amount);
                    else if (unit == null && _attackerStartingUnits[i].Item1 != null)
                        attackerCasualties[i] = (unit.Model, _attackerStartingUnits[i].Item2);
                }
                (UnitModel, int)[] defenderCasualties = new (UnitModel, int)[7];
                for (int i = 0; i < _defender.Units.Length; i++)
                {
                    UnitInstance unit = _defender.Units[i];
                    if (unit != null)
                        defenderCasualties[i] = (unit.Model, _defenderStartingUnits[i].Item2 - unit.Amount);
                    else if (unit == null && _defenderStartingUnits[i].Item1 != null)
                        defenderCasualties[i] = (unit.Model, _defenderStartingUnits[i].Item2);
                }

                ui.Init(attackerWon, () => Exit(attackerWon), _attacker.Portrait,
                    _defender.Portrait, _attacker.Name, _defender.Name,
                    attackerCasualties, defenderCasualties);
                break;
            }
        }
    }

    private void Exit(bool attackerWon)
    {
        LevelManager.CurrentLevel.BackFromCombat(attackerWon);
        SceneManager.UnloadScene("Combat");
    }

    private IEnumerator UnitAIRoutine()
    {
        var cell = _ai.GetCellClosestToUnit(_currentUnit);
        yield return StartCoroutine(GoToTargetRoutine(cell));
    }

    private IEnumerator PlayerUnitTurnRoutine()
    {
        var ienum = GetUnitCommandRoutine();
        yield return StartCoroutine(ienum);
        var pos = (Vector3Int)ienum.Current;
        yield return StartCoroutine(GoToTargetRoutine(pos));
    }

    private IEnumerator GoToTargetRoutine(Vector3Int pos)
    {
        var opposition = AreOpposingUnits(_currentUnit, pos);
        if (opposition != null)
        {
            if (_currentUnit.Model.IsRanged)
                yield return StartCoroutine(AttackRoutine(_currentUnit, opposition));
            else if (GetAttackPosition(_currentUnit, opposition) != null)
            {
                yield return StartCoroutine(MoveUnitRoutine(_currentUnit, GetAttackPosition(_currentUnit, opposition).Value));
                yield return StartCoroutine(AttackRoutine(_currentUnit, opposition));
            }
        }
        else
            yield return StartCoroutine(MoveUnitRoutine(_currentUnit, pos));
    }

    private Vector3Int? GetAttackPosition(UnitInstance attacker, UnitInstance defender)
    {
        var neighbors = _pathfinder.GetNeighbors(defender.CombatCellPosition);
        if (neighbors.Contains(attacker.CombatCellPosition))
            return attacker.CombatCellPosition;

        List<Vector3Int> possibilities = new();
        foreach (var neighbor in neighbors)
        {
            if (_ui.GetTile(neighbor) != null)
            {
                possibilities.Add(neighbor);
            }
        }

        if (possibilities.Count > 0)
        {
            var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var mouseTile = _ui.WorldToCell(mousePos);
            mouseTile.z = 0;
            return possibilities.Aggregate((n1, n2) =>
                (mousePos - n2).magnitude < (mousePos - n1).magnitude ? n2 : n1);
        }
        else
            return null;
    }

    private IEnumerator AttackRoutine(UnitInstance attacker, UnitInstance defender)
    {
        attacker.Attack(defender);
        yield return new WaitForSeconds(0.75f);
    }

    private void RemoveAllWalkables()
    {
        for (int i = XMin; i <= XMax; i++)
        {
            for (int j = YMin; j <= YMax; j++)
            {
                _ui.SetTile(new Vector3Int(i, j, 0), null);
            }
        }
    }

    private IEnumerator MoveUnitRoutine(UnitInstance unit, Vector3Int end)
    {
        var path = _pathfinder.CalculatePath(unit.CombatCellPosition, end);
        if (path == null || path.Count == 0)
            yield break;

        for (int i = 1; i < path.Count; i++)
        {
            unit.CombatCellPosition = path[i].Position;
            unit.CombatWorldPosition = _walkable.CellToWorld(unit.CombatCellPosition);
            yield return new WaitForSeconds(unit.Model.HexAnimationSpeed);
        }

        yield return new WaitForSeconds(unit.Model.HexAnimationSpeed);
    }

    private IEnumerator GetUnitCommandRoutine()
    {
        Vector3Int pos;
        while (!TraversalInteractions.CilckedOnMap(out pos, _camera, _ui))
        {
            if (Input.GetMouseButton(0) && AreOpposingUnits(_currentUnit, pos) != null)
                break;

            yield return null;
        }

        yield return pos;
    }

    private UnitInstance AreOpposingUnits(UnitInstance unit, Vector3Int otherUnit)
    {
        var unitClicked = _turnOrder.FirstOrDefault(u => u.CombatCellPosition == otherUnit);
        if (unitClicked != null)
        {
            // i.e. opposite unit clicked
            if ((_attacker.Units.Contains(_currentUnit) && !_attacker.Units.Contains(unitClicked))
                || (_attacker.Units.Contains(unitClicked) && !_attacker.Units.Contains(_currentUnit)))
            {
                return unitClicked;
            }
        }

        return null;
    }

    private void ShowUnitWalkables(UnitInstance unit)
    {
        var posXMin = unit.CombatCellPosition.x - unit.Model.Speed;
        var posXMax = unit.CombatCellPosition.x + unit.Model.Speed;
        var posYMin = unit.CombatCellPosition.y - unit.Model.Speed;
        var posYMax = unit.CombatCellPosition.y + unit.Model.Speed;

        for (int i = posXMin; i <= posXMax; i++)
        {
            for (int j = posYMin; j <= posYMax; j++)
            {
                var pos = new Vector3Int(i, j);

                if (Mathf.Abs(unit.CombatCellPosition.x - i) + Mathf.Abs(unit.CombatCellPosition.y - j) > unit.Model.Speed + 1)
                    continue;
                if (_obstacles.GetTile(pos) != null)
                    continue;
                if (_turnOrder.Any(u => u.CombatCellPosition == pos))
                    continue;

                if (_walkable.GetTile(pos) != null)
                    _ui.SetTile(pos, MovableTile);
            }
        }
    }

    private void InitializeUnits(ICombatant combatant, UnitCombatView[] combatViews, Vector3Int[] positions, bool flip)
    {
        for (int i = 0; i < combatant.Units.Length; i++)
        {
            UnitInstance unit = combatant.Units[i];

            if (unit == null)
                continue;

            unit.CombatCellPosition = positions[i];
            unit.CombatWorldPosition = _walkable.CellToWorld(positions[i]);
            combatViews[i] = Instantiate(unit.Model.p_combatView);
            combatViews[i].Init(unit, flip);
            combatViews[i].name = combatant.Name + "_" + i;
        }
    }

    private bool SameCombatant(UnitInstance u1, UnitInstance u2)
    {
        if ((_attacker.Units.Contains(u1) && _attacker.Units.Contains(u2))
            || (_defender.Units.Contains(u1) && !_defender.Units.Contains(u2)))
        {
            return true;
        }
        else
            return false;
    }

#if UNITY_EDITOR


    [ContextMenu("Attacker Win")]
    private void AttackerAutoWin()
    {
        foreach (var unit in _defender.Units)
        {
            if (unit != null)
                unit.Die();
        }
    }

#endif

    private class UnitAI
    {
        CombatManager _cm;

        private bool IsAttackerUnit(UnitInstance unit) => _cm._attacker.Units.Contains(unit);

        public UnitAI(CombatManager cm)
        {
            _cm = cm;
        }

        public Vector3Int GetCellClosestToUnit(UnitInstance unit)
        {
            var target = FindTarget(unit);

            Vector3Int[] positions = GetValidPositions(unit);
            return GetClosestToTarget(positions, target);
        }

        private Vector3Int GetClosestToTarget(Vector3Int[] positions, UnitInstance target)
        {
            return positions.Aggregate((p1, p2) =>
                (p1 - target.CombatCellPosition).magnitude < (p2 - target.CombatCellPosition).magnitude ?
                p1 : p2);
        }

        private Vector3Int[] GetValidPositions(UnitInstance unit)
        {
            var ret = new List<Vector3Int>();

            var posXMin = unit.CombatCellPosition.x - unit.Model.Speed;
            var posXMax = unit.CombatCellPosition.x + unit.Model.Speed;
            var posYMin = unit.CombatCellPosition.y - unit.Model.Speed;
            var posYMax = unit.CombatCellPosition.y + unit.Model.Speed;

            for (int i = posXMin; i <= posXMax; i++)
            {
                for (int j = posYMin; j <= posYMax; j++)
                {
                    var pos = new Vector3Int(i, j);

                    if (Mathf.Abs(unit.CombatCellPosition.x - i) + Mathf.Abs(unit.CombatCellPosition.y - j) > unit.Model.Speed + 1)
                        continue;
                    if (_cm._obstacles.GetTile(pos) != null)
                        continue;
                    if (_cm._turnOrder.Any(u => u.CombatCellPosition == pos && _cm.SameCombatant(u, unit)))
                        continue;

                    if (_cm._walkable.GetTile(pos) != null)
                        ret.Add(pos);
                }
            }

            return ret.ToArray();
        }

        private UnitInstance FindTarget(UnitInstance unit)
        {
            if (IsAttackerUnit(unit))
                return FindTarget(unit, _cm._defender.Units);
            else
                return FindTarget(unit, _cm._attacker.Units);
        }

        private UnitInstance FindTarget(UnitInstance toUnit, UnitInstance[] fromUnits)
        {
            UnitInstance closest = null;
            foreach (var unit in fromUnits)
            {
                if (unit == null)
                    continue;
                if (closest == null)
                    closest = unit;

                if ((unit.CombatCellPosition - toUnit.CombatCellPosition).magnitude <
                    (closest.CombatCellPosition - toUnit.CombatCellPosition).magnitude)
                {
                    closest = unit;
                }
            }

            return closest;
        }
    }
}