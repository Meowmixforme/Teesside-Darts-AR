// GameModes/IDartGameMode.cs
public interface IDartGameMode
{
    void InitializeGame();
    bool ProcessScore(int baseNumber, int multiplier);
    bool CheckWinCondition();
    string GetScoreDisplay();
    int GetMaxThrows();
    int GetThrowsLeft();
    int GetTotalThrowsLeft();
    void StartNewTurn();
    bool IsDoubleOutRequired();  // Added method
    bool WouldFinishGame(int points);  // Added method
}