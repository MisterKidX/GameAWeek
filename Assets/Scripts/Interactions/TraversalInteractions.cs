using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    Tilemap _metadata;

    [SerializeField]
    TileMapAstar _astar;
    [SerializeField]
    TraversalUI _traversalUI;
    [SerializeField]
    private float _cameraScrollSpeed;
    [SerializeField]
    AudioSource _sfx;
    [SerializeField]
    AudioClip _heroMovement;

    private List<PathPoint> _path = null;
    public void Update()
    {
        Vector3Int cellPos;

        SetMouseCursor();

        if (CilckedOnMap(out cellPos) && LevelManager.CurrentLevel.CurrentPlayer.HasHeroSelected &&
            ((_path == null || _path.Count == 0) || cellPos != _path[_path.Count - 1].Position))
        {
            if (_sfx.clip == _heroMovement)
                _sfx.Stop();

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
                _sfx.clip = _heroMovement;
                _sfx.volume = 0.2f;
                _sfx.Play();
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
        bool openCastle = false;
        _camController.StickToObject(LevelManager.CurrentLevel.CurrentPlayer.SelectedHero.View.gameObject);

        for (int i = 1; i < _path.Count; i++)
        {
            if (i == _path.Count - 1)
            {
                if (_sfx.clip == _heroMovement)
                    _sfx.Stop();

                if (!CanMove_DestinationAsNeutralUnit())
                    break;
                if (!CanMove_DestinationAsAquirable())
                    break;
                else if (!CanMove_DestinationAsAnotherHero())
                    break;
                else if (!CanMove_DestinationAsCastle(selectedHero))
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

        if (_sfx.clip == _heroMovement)
            _sfx.Stop();

        _path = null;
        _camController.UnstickToObject();
    }

    private bool CanMove_DestinationAsCastle(HeroInstance hero)
    {
        var destination = _path[_path.Count - 1].Position;
        CastleInstance[] castles = LevelManager.CurrentLevel.GetAllCastles();

        var castle = castles.FirstOrDefault(c => c.Position == _grid.CellToWorld(destination));
        if (castle == null)
            return true;

        // i.e., players castle
        if (castle.Holder.Heroes.Contains(hero))
        {
            hero.Position = destination;
            LevelManager.CurrentLevel.OpenCastleUIView(castle);
            return true;
        }
        else
        {
            bool hasUnits = false;
            foreach (var unit in castle.Units)
            {
                if (unit != null)
                {
                    hasUnits = true;
                    break;
                }
            }

            if (hasUnits)
                LevelManager.CurrentLevel.EnterCombat(hero, castle);
            else
                LevelManager.CurrentLevel.GiveCastleTo(castle, hero.Holder);
        }

        return true;
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

            return false;
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
        var aquirable = _obstacles.GetTile(_path[_path.Count - 1].Position) as AquirableTile;
        if (aquirable != null)
        {
            aquirable.Aquire(LevelManager.CurrentLevel.CurrentPlayer);
            var instObject = _obstacles.GetInstantiatedObject(_path[_path.Count - 1].Position);
            if (instObject != null)
            {
                var mineView = instObject.GetComponent<ResourceGeneratorView>();
                if (mineView != null)
                    mineView.Capture(LevelManager.CurrentLevel.CurrentPlayer);
            }
            if (!aquirable.CanStepOver)
            {
                _obstacles.SetTile(_path[_path.Count - 1].Position, null);
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

    private void SetMouseCursor()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 0;
        var worldPoint = _camController.Camera.ScreenToWorldPoint(mousePos);
        var pos = _grid.WorldToCell(worldPoint);

        var ground = _ground.GetTile(pos);
        var placement = _obstacles.GetTile(pos);
        var metadata = _metadata.GetTile(pos);
        var threat = metadata as ThreatTile;
        var castle = metadata as CastleEntranceTile;
        var aquirable = placement as AquirableTile;

        if (threat != null)
            GameLogic.ChangeCursor(CursorIcon.EnterCombat);
        else if (castle != null)
            GameLogic.ChangeCursor(CursorIcon.EnterCastle);
        else if (aquirable != null)
            GameLogic.ChangeCursor(CursorIcon.HeroAquirable);
        else if (placement != null)
            GameLogic.ChangeCursor(CursorIcon.Regular);
        else if (ground != null)
            GameLogic.ChangeCursor(CursorIcon.HeroMove);
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
