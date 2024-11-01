using UnityEngine;

public class HeroView : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _colorer;
    [SerializeField]
    Transform _pivot;

    HeroInstance _instance;

    public void Init(HeroInstance instance)
    {
        _instance = instance;
        PaintColor(_instance.Holder.Color);
        transform.position = _instance.Position;
    }

    private void PaintColor(Color color)
    {
        _colorer.color = color;
    }

    Vector3 delta = Vector3.negativeInfinity;
    private void LateUpdate()
    {
        delta = _instance.Position - transform.position;

        if (delta.sqrMagnitude > .4f)
        {
            var dot = Vector3.Dot(Vector3.right, delta);
            if (dot > 0 && dot <= 1)
                _pivot.localScale = new Vector3(-1, 1, 1);
            else
                _pivot.localScale = Vector3.one;
        }

        transform.position = _instance.Position;
    }
}