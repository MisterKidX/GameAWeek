using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Camera Camera { get; private set; }

    private void Awake() => Camera = GetComponent<Camera>();

    internal void StickToObject(GameObject go)
    {
        StartCoroutine(StickToObjectRoutine(go));
    }

    internal void UnstickToObject()
    {
        StopAllCoroutines();
    }

    private IEnumerator StickToObjectRoutine(GameObject go)
    {
        while (true)
        {
            PointAt(go);
            yield return null;
        }
    }

    internal void PointAt(GameObject go)
    {
        var pos = Camera.transform.position;
        pos.x = go.transform.position.x;
        pos.y = go.transform.position.y;
        Camera.transform.position = pos;
    }
}
