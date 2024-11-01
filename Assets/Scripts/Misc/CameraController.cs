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
        UnstickToObject();
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
            yield return null;
            yield return new WaitForEndOfFrame();
            PointAt(go);
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
