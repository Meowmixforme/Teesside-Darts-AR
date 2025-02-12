using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Cricket game

public class CricketGame : BaseDartGame
{
    private Dictionary<int, int> cricketHits = new Dictionary<int, int>();
    private Dictionary<int, int> cricketScores = new Dictionary<int, int>();
    private readonly int[] cricketNumbers = { 15, 16, 17, 18, 19, 20, 25 };
    private const int MARKS_TO_CLOSE = 3;

    public override void InitializeGame()
    {
        cricketHits.Clear();
        cricketScores.Clear();
        foreach (int number in cricketNumbers)
        {
            cricketHits[number] = 0;
            cricketScores[number] = 0;
        }
        totalThrowsLeft = GetMaxThrows();
        throwsLeft = 3;
    }

    public override bool ProcessScore(int baseNumber, int multiplier)
    {
        if (!cricketNumbers.Contains(baseNumber))
        {
            throwsLeft--;
            totalThrowsLeft--;
            return false;
        }

        int currentHits = cricketHits[baseNumber];
        cricketHits[baseNumber] = Mathf.Min(MARKS_TO_CLOSE, currentHits + multiplier);

        if (cricketHits[baseNumber] >= MARKS_TO_CLOSE)
        {
            int extraHits = (currentHits + multiplier) - MARKS_TO_CLOSE;
            if (extraHits > 0)
            {
                cricketScores[baseNumber] += baseNumber * extraHits;
            }
        }

        throwsLeft--;
        totalThrowsLeft--;
        return true;
    }

    public override bool CheckWinCondition()
    {
        return cricketNumbers.All(number => cricketHits[number] >= MARKS_TO_CLOSE);
    }

    public override string GetScoreDisplay()
    {
        string display = "Cricket Scores:\n";
        foreach (int number in cricketNumbers)
        {
            string marks = new string('/', cricketHits[number]);
            string closed = cricketHits[number] >= MARKS_TO_CLOSE ? " CLOSED" : "";
            display += $"{number}: {marks}{closed} - {cricketScores[number]} pts\n";
        }
        return display;
    }

    public override int GetMaxThrows()
    {
        return 40;
    }

    public int GetTotalScore()
    {
        return cricketNumbers.Sum(number => cricketScores[number]);
    }
}