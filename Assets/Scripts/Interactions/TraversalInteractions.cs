using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TraversalInteractions : MonoBehaviour
{
    [SerializeField]
    Camera _camera;
    [SerializeField]
    Grid _grid;
    [SerializeField]
    Tilemap _ground;
    [SerializeField]
    Tilemap _obstacles;
    [SerializeField]
    private float _cameraScrollSpeed;

    public void Update()
    {
        if (CilckedOnMap(out Vector3Int cellPos) && LevelManager.CurrentLevel.CurrentPlayer.HasHeroSelected)
        {
            TileMapAstar.CalculatePath(LevelManager.CurrentLevel.CurrentPlayer.SelectedHero.Position, cellPos);
        }
        if (MoveMapUp())
            _camera.transform.Translate(Vector3.up * Time.deltaTime * _cameraScrollSpeed);
        if (MoveMapDown())
            _camera.transform.Translate(Vector3.down * Time.deltaTime * _cameraScrollSpeed);
        if (MoveMapLeft())
            _camera.transform.Translate(Vector3.left * Time.deltaTime * _cameraScrollSpeed);
        if (MoveMapRight())
            _camera.transform.Translate(Vector3.right * Time.deltaTime * _cameraScrollSpeed);
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
        if (Input.GetKey(KeyCode.D))
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
        var worldPoint = _camera.ScreenToWorldPoint(mousePos);
        pos = _grid.WorldToCell(worldPoint);
        var tile = _ground.GetTile(pos);

        if (tile == null) return false;

        return true;
    }
}
