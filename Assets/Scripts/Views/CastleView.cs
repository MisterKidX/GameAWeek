using UnityEngine;

public class CastleView : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer[] _colorers;
    CastleInstance _instance;
    public void Init(CastleInstance instance)
    {
        _instance = instance;
        transform.position = _instance.Position;
        PaintColor(_instance.Holder.Color);
    }

    private void Update()
    {
        PaintColor(_instance.Holder.Color);
    }

    private void PaintColor(Color color)
    {
        foreach (var item in _colorers)
            item.color = color;
    }
}