using UnityEngine;

public class ResourceGeneratorView : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _flag;

    public ResourceGeneratorInstance Instance;
    public void Init(ResourceGeneratorInstance instance)
    {
        Instance = instance;
    }

    private void Update()
    {
        if (Instance == null)
            return;

        if (Instance.Holder != null)
            _flag.color = Instance.Holder.Color;
        else
            _flag.color = Color.grey;
    }

    public void Capture(PlayerInstace player)
    {
        Instance.Holder = player;
    }
}