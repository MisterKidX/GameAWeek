using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    Vector3 _target;
    public void Init(Vector3 target)
    {
        _target = target;
    }

    Vector3 _ref;
    private void Update()
    {
        if (_target == null)
            return;

        if ((transform.position - _target).magnitude < 0.15f)
            Destroy(gameObject);

        transform.position = Vector3.SmoothDamp(transform.position, _target, ref _ref, 0.1f);
    }
}