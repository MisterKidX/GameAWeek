using UnityEngine;

public class IncomeGeneratorBuildingInstance : IncomeGeneratorBuildingModel, ITimeableReactor
{
    [field: SerializeField]
    public int ReactionTime { get; private set; }

    public void React(int days)
    {
        // currentPlayer.Resources.First(r => r.Model == Resource).Amount += Income;
    }

    private void Awake() => (this as ITimeableReactor).EnlistToTimeManager();
}
