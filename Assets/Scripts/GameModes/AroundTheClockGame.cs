using UnityEngine;

// aroundtheclock game
public class AroundTheClockGame : BaseDartGame
{
    public override void InitializeGame()
    {
        currentScore = 1;  // Start at 1
        totalThrowsLeft = GetMaxThrows();
        throwsLeft = 3;
    }

    public override bool ProcessScore(int baseNumber, int multiplier)
    {
        // In Around the Clock, we only care about hitting the right number,
        // not the multiplier
        if (baseNumber == currentScore)
        {
            currentScore++;
            throwsLeft--;
            totalThrowsLeft--;
            return true;
        }
        throwsLeft--;
        totalThrowsLeft--;
        return false;
    }

    public override bool CheckWinCondition()
    {
        return currentScore > 20;  // Game ends after hitting 20
    }

    public override string GetScoreDisplay()
    {
        return $"Current Target: {currentScore}";
    }

    public override int GetMaxThrows()
    {
        return 30;
    }
}