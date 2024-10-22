public abstract class AquirableTile : BaseTile 
{
    public bool CanStepOver = true;

    public abstract void Aquire(PlayerInstace player);
}