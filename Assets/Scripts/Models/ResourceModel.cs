using UnityEngine;

[CreateAssetMenu(fileName = "ResourceModel", menuName = "Models/Resource")]
public class ResourceModel : ScriptableObject
{
    public Sprite Icon;
    public string Name;

    public ResourceInstance Create(int amount)
    {
        var inst = ScriptableObject.CreateInstance<ResourceInstance>();
        inst.Init(this, amount);

        return inst;
    }
}
