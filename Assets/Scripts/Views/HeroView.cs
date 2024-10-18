using UnityEngine;

public class HeroView : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _colorer;
    HeroInstance _instance;
    
    public void Init(HeroInstance instance)
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