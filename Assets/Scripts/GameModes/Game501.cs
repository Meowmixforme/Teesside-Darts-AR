using UnityEngine;

public class Game501 : BaseDartGame
{
    private bool mustDoubleOut = true;

    public override void InitializeGame()
    {
        currentScore = 501;
        totalThrowsLeft = GetMaxThrows();
        throwsLeft = 3;
    }

    public override bool ProcessScore(int baseNumber, int multiplier)
    {
        int points = baseNumber * multiplier;

        // Check if score would go below 0
        if (currentScore - points < 0)
            return false;

        // Check double-out rule
        if (mustDoubleOut && currentScore - points == 0)
        {
            bool isDouble = points == 50 || // Double Bull
                           (multiplier == 2); // Regular double
            if (!isDouble)
                return false;
        }

        currentScore -= points;
        throwsLeft--;
        totalThrowsLeft--;
        return true;
    }

    public override bool CheckWinCondition()
    {
        return currentScore == 0;
    }

    public override string GetScoreDisplay()
    {
        return $"Score: {currentScore}";
    }

    public override int GetMaxThrows()
    {
        return 50;
    }

    public override bool IsDoubleOutRequired()
    {
        return mustDoubleOut;
    }

    public override bool WouldFinishGame(int points)
    {
        return (currentScore - points) == 0;
    }
}