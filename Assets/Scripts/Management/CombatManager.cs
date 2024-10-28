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
    HeroInstance _defender;

    Vector3Int[] _leftPositions;
    Vector3Int[] _rightPositions;

    //private void Update()
    //{
    //    var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    var cell = _walkable.WorldToCell(mousePos);
    //    Debug.Log(cell);
    //}

    public void Init(HeroInstance attacker, HeroInstance defender)
    {
        _attacker = attacker;
        _defender = defender;
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
            OrderBy(u => u.Model.Speed).
            ToList();

        StartCoroutine(CombatSequenceRoutine());
    }

    private IEnumerator CombatSequenceRoutine()
    {
        _currentUnit.Selected = true;

        ShowUnitWalkables(_currentUnit);

        yield return null;

        _currentUnitIndex++;
    }

    private void ShowUnitWalkables(UnitInstance unit)
    {
        var posXMin = unit.CombatCellPosition.x - unit.Model.Speed;
        var posXMax = unit.CombatCellPosition.x + unit.Model.Speed;
        var posYMin = unit.CombatCellPosition.y - unit.Model.Speed;
        var posYMax = unit.CombatCellPosition.y + unit.Model.Speed;

        for (int i = posXMin; i <= posXMax; i++)
        {
            for (int j = posYMin; j < posYMax; j++)
            {
                var pos = new Vector3Int(i, j);

                if (Mathf.Abs(unit.CombatCellPosition.x - i) + Mathf.Abs(unit.CombatCellPosition.y - j) > unit.Model.Speed + 1)
                    continue;
                if (_obstacles.GetTile(pos) != null)
                    continue;

                if (_walkable.GetTile(pos) != null)
                {
                    _ui.SetTile(pos, MovableTile);
                }
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
