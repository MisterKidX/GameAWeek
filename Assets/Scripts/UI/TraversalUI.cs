using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraversalUI : MonoBehaviour
{
    [SerializeField]
    Grid _grid;
    [SerializeField]
    GameObject p_Arrow;
    [SerializeField]
    GameObject p_X;

    List<GameObject> _pathVisuals = new List<GameObject>();

    public void VisualizePath(List<PathPoint> path, float movementAllowance)
    {
        Reset();

        if (path != null && path.Count > 0)
        {
            for (int i = 1; i < path.Count - 1; i++)
            {
                var d = path[i + 1].Position - path[i].Position;
                var arrow = Instantiate(p_Arrow);
                _pathVisuals.Add(arrow);

                if (path[i].AccumulatedMoveCost > movementAllowance)
                    arrow.GetComponentInChildren<Image>().color = Color.red;

                if (d.x == 0)
                {
                    if (d.y == 1)
                        arrow.transform.Rotate(0, 0, 180);
                }
                else if (d.y == 0)
                {
                    if (d.x == 1)
                        arrow.transform.Rotate(0, 0, 90);
                    if (d.x == -1)
                        arrow.transform.Rotate(0, 0, -90);
                }
                else if (d.x == 1)
                {
                    if (d.y == 1)
                        arrow.transform.Rotate(0, 0, 135);
                    if (d.y == -1)
                        arrow.transform.Rotate(0, 0, 45);
                }
                else if (d.x == -1)
                {
                    if (d.y == 1)
                        arrow.transform.Rotate(0, 0, -135);
                    if (d.y == -1)
                        arrow.transform.Rotate(0, 0, -45);
                }

                arrow.transform.position = _grid.CellToWorld(path[i].Position) + new Vector3(0.5f, 0.5f, 0);
            }

            var x = Instantiate(p_X);
            x.transform.position = _grid.CellToWorld(path[path.Count - 1].Position) + new Vector3(0.5f, 0.5f, 0);
            _pathVisuals.Add(x);
            if (path[path.Count-1].AccumulatedMoveCost > movementAllowance)
                x.GetComponentInChildren<Image>().color = Color.red;
        }
    }

    public void Reset()
    {
        if (_pathVisuals.Count > 0)
        {
            while (_pathVisuals.Count != 0)
            {
                Destroy(_pathVisuals[0]);
                _pathVisuals.RemoveAt(0);
            }
        }
    }

    internal void ConsumePath()
    {
        Destroy(_pathVisuals[0]);
        _pathVisuals.RemoveAt(0);
    }
}
