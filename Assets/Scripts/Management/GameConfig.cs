public static class GameConfig
{
    public const int MovementBarMaxMovementPoints = 30;

    static ResourceModel[] _resources;
    public static ResourceModel[] Resources 
    { 
        get
        {
            if (_resources == null)
                _resources = UnityEngine.Resources.LoadAll<ResourceModel>("");

            return _resources;
        } 
    }

    public static ResourceInstance[] GetStartingResouces()
    {
        var models = Resources;
        var insts = new ResourceInstance[models.Length];

        for (int i = 0; i < models.Length; i++)
        {
            ResourceModel rm = models[i];
            switch (rm.Name)
            {
                case "Gold":
                    insts[i] = rm.Create(10_000);
                    break;
                default:
                    insts[i] = rm.Create(10);
                    break;
            }
        }

        return insts;
    }
}