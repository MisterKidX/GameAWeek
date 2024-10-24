public interface ITimeableReactor
{
    public int ReactionTime { get; }

    public void React(int totalDays);

    public void EnlistToTimeManager() => LevelManager.CurrentLevel.EnlistTimeObject(this);
}