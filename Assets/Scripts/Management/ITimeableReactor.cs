public interface ITimeableReactor
{
    public int ReactionTime { get; }

    public void React(int days);

    public void EnlistToTimeManager() => LevelManager.CurrentLevel.EnlistTimeObject(this);
}