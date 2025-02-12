// GameModes/BaseDartGame.cs
public abstract class BaseDartGame : IDartGameMode
{
    protected int currentScore;
    protected int throwsLeft = 3;
    protected int totalThrowsLeft;

    public abstract void InitializeGame();
    public abstract bool ProcessScore(int baseNumber, int multiplier);
    public abstract bool CheckWinCondition();
    public abstract string GetScoreDisplay();
    public abstract int GetMaxThrows();

    public int GetThrowsLeft() => throwsLeft;
    public int GetTotalThrowsLeft() => totalThrowsLeft;

    public virtual void StartNewTurn()
    {
        throwsLeft = 3;
    }

    public virtual bool IsDoubleOutRequired()
    {
        return false;
    }

    public virtual bool WouldFinishGame(int points)
    {
        return false;
    }
}