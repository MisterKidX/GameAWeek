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

    HeroInstance _attacker;
    (UnitModel, int)[] _attackerStartingUnits = new (UnitModel, int)[7];
    HeroInstance _defender;
    (UnitModel, int)[] _defenderStartingUnits = new (UnitModel, int)[7];

    Vector3Int[] _leftPositions;
    Vector3Int[] _rightPositions;

    public void Init(HeroInstance attacker, HeroInstance defender)
    {
        _attacker = attacker;
        _defender = defender;

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
            yield return UnitTurnRoutine();
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

                ui.Init(attackerWon, Exit,_attacker.Model.Portrait,
                    _defender.Model.Portrait, _attacker.Model.Name, _defender.Model.Name,
                    attackerCasualties, defenderCasualties);
                break;
            }
        }
    }

    private void Exit()
    {
        LevelManager.CurrentLevel.BackFromCombat();
        SceneManager.UnloadSceneAsync("Combat");
    }

    private IEnumerator UnitTurnRoutine()
    {
        RemoveAllWalkables();
        ShowUnitWalkables(_currentUnit);
        var ienum = GetUnitCommandRoutine();
        yield return StartCoroutine(ienum);
        var pos = (Vector3Int)ienum.Current;
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

    private void InitializeUnits(HeroInstance hero, UnitCombatView[] combatViews, Vector3Int[] positions, bool flip)
    {
        for (int i = 0; i < hero.Units.Length; i++)
        {
            UnitInstance unit = hero.Units[i];

            if (unit == null)
                continue;

            unit.CombatCellPosition = positions[i];
            unit.CombatWorldPosition = _walkable.CellToWorld(positions[i]);
            combatViews[i] = Instantiate(unit.Model.p_combatView);
            combatViews[i].Init(unit, flip);
            combatViews[i].name = hero.Holder.Name + "_" + i;
        }
    }

#if UNITY_EDITOR

    //private void OnGUI()
    //{
    //    if (_attacker != null)
    //        return;

    //    GUIStyle g = new GUIStyle(GUI.skin.button);
    //    g.fontSize = 72;

    //    Vector2 size = new Vector2(500, 150);
    //    Rect r1 = new Rect(Screen.width / 2f - size.x / 2f, Screen.height / 2f + size.y / 2f, size.x, size.y);

    //    if (GUI.Button(r1, "Quick Load", g))
    //    {
    //        StartCoroutine(EDITOR_LoadRoutine());
    //    }
    //}

    private IEnumerator EDITOR_LoadRoutine()
    {
        SceneManager.LoadScene("DemoLevel", LoadSceneMode.Additive);
        yield return new WaitForSeconds(.5f);

        var scene = SceneManager.GetSceneByName("DemoLevel");
        var root = scene.GetRootGameObjects();

        foreach (var go in root)
        {
            if (go.TryGetComponent(out LevelManager levelManager))
            {
                levelManager.EDITORONLY_LoadCombat();
            }
        }
    }

#endif
}
