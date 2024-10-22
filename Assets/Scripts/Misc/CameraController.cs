using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Camera Camera { get; private set; }
    private GameObject _stick;

    private void Awake() => Camera = GetComponent<Camera>();

    internal void StickToObject(GameObject go)
    {
        _stick = go;
        StartCoroutine(StickToObjectRoutine());
    }

    internal void UnstickToObject()
    {
        StopAllCoroutines();
        _stick = null;
    }

    private IEnumerator StickToObjectRoutine()
    {
        while (true)
        {
            var pos = Camera.transform.position;
            pos.x = _stick.transform.position.x;
            pos.y = _stick.transform.position.y;
            Camera.transform.position = pos;
            yield return null;
        }
    }
}
