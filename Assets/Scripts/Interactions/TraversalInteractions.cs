using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TraversalInteractions : MonoBehaviour
{
    [SerializeField]
    CameraController _camController;
    [SerializeField]
    Grid _grid;
    [SerializeField]
    Tilemap _ground;
    [SerializeField]
    Tilemap _obstacles;
    [SerializeField]
    TileMapAstar _astar;
    [SerializeField]
    TraversalUI _traversalUI;
    [SerializeField]
    private float _cameraScrollSpeed;

    private List<PathPoint> _path = null;
    public void Update()
    {
        Vector3Int cellPos;

        if (CilckedOnMap(out cellPos) && LevelManager.CurrentLevel.CurrentPlayer.HasHeroSelected &&
            ((_path == null || _path.Count == 0) || cellPos != _path[_path.Count - 1].Position))
        {
            StopAllCoroutines();
            _camController.UnstickToObject();
            _path = _astar.CalculatePath(LevelManager.CurrentLevel.CurrentPlayer.SelectedHero.Position, cellPos);
            _traversalUI.VisualizePath(_path, LevelManager.CurrentLevel.CurrentPlayer.SelectedHero.RemainingMovementPoints);
        }
        else if (CilckedOnMap(out cellPos) && LevelManager.CurrentLevel.CurrentPlayer.HasHeroSelected && _path != null)
        {
            // if the cell position which was clicked is the X
            if (cellPos == _path[_path.Count - 1].Position)
            {
                StartCoroutine(MoveSelectedHero(LevelManager.CurrentLevel.CurrentPlayer.SelectedHero));
            }
        }

        if (MoveMapUp())
            _camController.Camera.transform.Translate(Vector3.up * Time.deltaTime * _cameraScrollSpeed);
        if (MoveMapDown())
            _camController.Camera.transform.Translate(Vector3.down * Time.deltaTime * _cameraScrollSpeed);
        if (MoveMapLeft())
            _camController.Camera.transform.Translate(Vector3.left * Time.deltaTime * _cameraScrollSpeed);
        if (MoveMapRight())
            _camController.Camera.transform.Translate(Vector3.right * Time.deltaTime * _cameraScrollSpeed);
    }

    public void Reset()
    {
        _path = null;
        _traversalUI.Reset();
    }

    private IEnumerator MoveSelectedHero(HeroInstance selectedHero)
    {
        _camController.StickToObject(LevelManager.CurrentLevel.CurrentPlayer.SelectedHero.View.gameObject);

        for (int i = 1; i < _path.Count; i++)
        {
            if (i == _path.Count -1)
            {
                if (!CanMove_DestinationAsAquirable())
                    break;
                if (!CanMove_DestinationAsAnotherHero())
                    break;
                if (!CanMove_DestinationAsNeutralUnit())
                    break;
            }

            // movement
            var moveCost = _path[i].MovemventCost;
            if (selectedHero.RemainingMovementPoints >= moveCost)
            {
                selectedHero.Position = _path[i].Position;
                selectedHero.RemainingMovementPoints -= moveCost;
                _traversalUI.ConsumePath();
            }
            else
                break;

            yield return new WaitForSeconds(GameConfig.Configuration.HeroMovementSpeed);
        }

        _path = null;
        _camController.UnstickToObject();
    }

    private bool CanMove_DestinationAsNeutralUnit()
    {
        var destination = _path[_path.Count - 1].Position;
        var neutrals = LevelManager.CurrentLevel.Neutrals;

        if (!neutrals.ContainsKey(destination))
            return true;
        else
        {
            LevelManager.CurrentLevel.
                EnterCombat(LevelManager.CurrentLevel.CurrentPlayer.SelectedHero, neutrals[destination]);
            return true;
        }
    }

    private bool CanMove_DestinationAsAnotherHero()
    {
        var destination = _path[_path.Count - 1].Position;
        var currentPlayerHeroes = LevelManager.CurrentLevel.CurrentPlayer.Heroes;

        foreach (var hero in currentPlayerHeroes)
        {
            // can't walk over your own hero
            // EXTRA: implement trading mechanic
            if (hero.Position == destination)
                return false;
        }

        foreach (var player in LevelManager.CurrentLevel.Players)
        {
            if (player == LevelManager.CurrentLevel.CurrentPlayer)
                continue;

            foreach (var hero in player.Heroes)
            {
                if (hero.Position == destination)
                {
                    LevelManager.CurrentLevel.EnterCombat(LevelManager.CurrentLevel.CurrentPlayer.SelectedHero, hero);
                    return false;
                }
            }
        }

        return true;
    }

    /// <returns> wether the hero can occupy the last tile</returns>
    private bool CanMove_DestinationAsAquirable()
    {
        var aquirable = _obstacles.GetTile(_path[_path.Count -1].Position) as AquirableTile;
        if (aquirable != null)
        {
            aquirable.Aquire(LevelManager.CurrentLevel.CurrentPlayer);
            var instObject = _obstacles.GetInstantiatedObject(_path[_path.Count -1].Position);
            if (instObject != null)
            {
                var mineView = instObject.GetComponent<MineView>();
                if (mineView != null)
                    mineView.Capture(LevelManager.CurrentLevel.CurrentPlayer);
            }
            if (!aquirable.CanStepOver)
            {
                _obstacles.SetTile(_path[_path.Count -1].Position, null);
                return false;
            }
        }

        return true;
    }

    private bool MoveMapRight()
    {
        if (Input.GetKey(KeyCode.D))
            return true;
        else
            return false;
    }

    private bool MoveMapLeft()
    {
        if (Input.GetKey(KeyCode.A))
            return true;
        else
            return false;
    }

    private bool MoveMapDown()
    {
        if (Input.GetKey(KeyCode.S))
            return true;
        else
            return false;
    }

    private bool MoveMapUp()
    {
        if (Input.GetKey(KeyCode.W))
            return true;
        else
            return false;
    }

    private bool CilckedOnMap(out Vector3Int pos)
    {
        pos = Vector3Int.zero;

        if (!Input.GetMouseButtonDown(0)) return false;

        var mousePos = Input.mousePosition;
        mousePos.z = 0;
        var worldPoint = _camController.Camera.ScreenToWorldPoint(mousePos);
        pos = _grid.WorldToCell(worldPoint);
        var tile = _ground.GetTile(pos);

        if (tile == null) return false;

        return true;
    }

    public static bool CilckedOnMap(out Vector3Int pos, Camera cam, Tilemap tilemap)
    {
        pos = Vector3Int.zero;

        if (!Input.GetMouseButtonDown(0)) return false;

        var mousePos = Input.mousePosition;
        mousePos.z = 0;
        var worldPoint = cam.ScreenToWorldPoint(mousePos);
        pos = tilemap.WorldToCell(worldPoint);
        pos.z = 0;
        var tile = tilemap.GetTile(pos);

        if (tile == null) return false;

        return true;
    }
}
