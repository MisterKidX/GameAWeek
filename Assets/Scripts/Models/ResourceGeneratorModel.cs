using UnityEngine;

[CreateAssetMenu(fileName = "ResourceGeneratorModel", menuName = "Models/Economy/Resource Generator")]
public class ResourceGeneratorModel : ScriptableObject
{
    public ResourceGeneratorView p_View;
    public int IncomeDelta = 1;
    public ResourceCost Resource;

    public ResourceGeneratorInstance Create(PlayerInstace holder)
    {
        var inst = ScriptableObject.CreateInstance<ResourceGeneratorInstance>();
        inst.Init(this, holder);

        return inst;
    }
}
