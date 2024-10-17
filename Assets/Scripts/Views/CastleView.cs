using UnityEngine;

public class CastleView : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _colorer;
    CastleInstance _instance;
    public void Init(CastleInstance instance)
    {
        _instance = instance;
        transform.position = _instance.Position;
        PaintColor(_instance.Holder.Color);
    }

    private void PaintColor(Color color)
    {
        _colorer.color = color;
    }
}