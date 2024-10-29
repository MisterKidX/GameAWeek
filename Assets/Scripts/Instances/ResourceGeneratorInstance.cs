using UnityEngine;

public class ResourceGeneratorInstance : ScriptableObject, ITimeableReactor
{
    public ResourceGeneratorModel Model;
    public PlayerInstace? Holder;

    public int ReactionTime => Model.IncomeDelta;

    private void Awake() => (this as ITimeableReactor).EnlistToTimeManager();

    public void React(int totalDays)
    {
        if (Holder == null)
            return;

        Holder.AddResource(Model.Resource.Resource, Model.Resource.Amount);
    }

    public void Init(ResourceGeneratorModel model, PlayerInstace holder)
    {
        Model = model;
        Holder = holder;
    }
}
