using UnityEngine;

public class ResourceInstance : ScriptableObject
{
    public ResourceModel Model;
    public int Amount { get; set; }

    public void Init(ResourceModel model, int amount)
    {
        Model = model;
        Amount = amount;
    }
}
