using System.Collections.Generic;
using UnityEngine;

public class TraversalUI : MonoBehaviour
{
    [SerializeField]
    Grid _grid;
    [SerializeField]
    GameObject p_Arrow;
    [SerializeField]
    GameObject p_X;

    public void VisualizePath(List<Vector3Int> path)
    {
        if (path != null && path.Count > 0)
        {
            for (int i = 1; i < path.Count - 1; i++)
            {
                var d = path[i + 1] - path[i];
                var arrow = Instantiate(p_Arrow);

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

                arrow.transform.position = _grid.CellToWorld(path[i]) + new Vector3(0.5f, 0.5f, 0);
            }

            var x = Instantiate(p_X);
            x.transform.position = _grid.CellToWorld(path[path.Count - 1]) + new Vector3(0.5f, 0.5f, 0);
        }
    }
}
