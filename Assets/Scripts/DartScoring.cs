using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class DartScoring : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text throwsLeftText;
    public TMP_Text gameStateText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverMessage;
    [SerializeField] private Button restartButton;

    public enum GameMode
    {
        Game501,
        Game301,
        AroundTheClock,
        Cricket
    }

    [Header("Game Settings")]
    [SerializeField] private GameMode currentGameMode = GameMode.Game501;
    private bool mustDoubleOut = true;

    // Standard dartboard number sequence (clockwise)
    private readonly int[] boardSequence = new int[]
    {
        20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5
    };

    // Game state variables
    private int currentScore;
    private int throwsLeft = 3;
    private bool isGameOver = false;
    private bool isDartboardSetup = false;
    private bool isTurnInProgress = false;

    // Cricket specific variables
    private Dictionary<int, int> cricketScores = new Dictionary<int, int>();
    private Dictionary<int, int> cricketHits = new Dictionary<int, int>();
    private readonly int[] cricketNumbers = new int[] { 15, 16, 17, 18, 19, 20, 25 }; // 25 represents bullseye
    private const int MARKS_TO_CLOSE = 3;

    // Game mode specific constants
    private const int MAX_THROWS_501 = 50;
    private const int MAX_THROWS_301 = 35;
    private const int MAX_THROWS_CLOCK = 30;
    private const int MAX_THROWS_CRICKET = 40;
    private int totalThrowsLeft;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        InitializeGame();
    }

    public void SetupDartboard(GameObject dartboard)
    {
        if (!isDartboardSetup)
        {
            Transform dartboardModel = dartboard.transform.Find("DartBoardModel");
            if (dartboardModel != null)
            {
                for (int i = 1; i <= 79; i++)
                {
                    Transform hitArea = dartboardModel.Find($"HitArea.{i:D3}");
                    if (hitArea != null)
                    {
                        hitArea.gameObject.tag = "dart_board";
                    }
                }
            }
            isDartboardSetup = true;
        }
    }

    private void InitializeGame()
    {
        switch (currentGameMode)
        {
            case GameMode.Game501:
                currentScore = 501;
                totalThrowsLeft = MAX_THROWS_501;
                break;
            case GameMode.Game301:
                currentScore = 301;
                totalThrowsLeft = MAX_THROWS_301;
                break;
            case GameMode.AroundTheClock:
                currentScore = 1;
                totalThrowsLeft = MAX_THROWS_CLOCK;
                break;
            case GameMode.Cricket:
                InitializeCricket();
                totalThrowsLeft = MAX_THROWS_CRICKET;
                break;
        }
        throwsLeft = 3;
        isGameOver = false;
        isTurnInProgress = false;
        if (gameStateText != null)
            gameStateText.text = "";
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        UpdateUI();
    }

    private void InitializeCricket()
    {
        cricketScores.Clear();
        cricketHits.Clear();
        foreach (int number in cricketNumbers)
        {
            cricketScores[number] = 0;
            cricketHits[number] = 0;
        }
    }

    public void ScoreDart(Collider hitCollider)
    {
        if (isGameOver || throwsLeft <= 0) return;

        string hitAreaStr = hitCollider.gameObject.name;
        if (!hitAreaStr.StartsWith("HitArea.")) return;

        int hitArea;
        if (!int.TryParse(hitAreaStr.Substring(8), out hitArea)) return;

        bool scored = false;
        switch (currentGameMode)
        {
            case GameMode.Game501:
            case GameMode.Game301:
                scored = Score501Game(hitArea);
                break;
            case GameMode.AroundTheClock:
                scored = ScoreAroundTheClock(hitArea);
                break;
            case GameMode.Cricket:
                scored = ScoreCricket(hitArea);
                break;
        }

        if (scored)
        {
            throwsLeft--;
            totalThrowsLeft--;
            UpdateUI();

            if (totalThrowsLeft <= 0 && !isGameOver)
            {
                GameOver("Out of throws! Game Over!");
            }
            else if (throwsLeft <= 0 && !isGameOver)
            {
                StartCoroutine(DelayedNewTurn());
            }
        }
    }

    private bool Score501Game(int hitArea)
    {
        int points = CalculateScore(hitArea);

        if (currentScore - points < 0)
        {
            if (gameStateText != null)
                gameStateText.text = "Bust!";
            return true;
        }

        if (mustDoubleOut && currentScore - points == 0)
        {
            bool isValidFinish = (hitArea >= 21 && hitArea <= 38) || hitArea == 60;
            if (!isValidFinish)
            {
                if (gameStateText != null)
                    gameStateText.text = "Must finish on a double!";
                return true;
            }
        }

        currentScore -= points;
        if (currentScore == 0)
        {
            GameWon();
        }
        return true;
    }

    private bool ScoreAroundTheClock(int hitArea)
    {
        if (hitArea >= 1 && hitArea <= 58)
        {
            int baseNumber = GetBaseNumber(hitArea);
            if (baseNumber == currentScore)
            {
                currentScore++;
                if (currentScore > 20)
                {
                    GameWon();
                }
                return true;
            }
        }
        return true;
    }

    private bool ScoreCricket(int hitArea)
    {
        int baseNumber = GetBaseNumber(hitArea);
        int multiplier = 1;

        if (hitArea >= 21 && hitArea <= 38) multiplier = 2;
        if (hitArea >= 39 && hitArea <= 58) multiplier = 3;

        if (hitArea == 59)
        {
            baseNumber = 25;
            multiplier = 1;
        }
        if (hitArea == 60)
        {
            baseNumber = 25;
            multiplier = 2;
        }

        if (cricketNumbers.Contains(baseNumber))
        {
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

            CheckCricketWin();
            return true;
        }
        return true;
    }

    private void CheckCricketWin()
    {
        bool allClosed = cricketNumbers.All(number => cricketHits[number] >= MARKS_TO_CLOSE);

        if (allClosed)
        {
            int totalScore = cricketNumbers.Sum(number => cricketScores[number]);
            GameWon();
            if (gameOverMessage != null)
            {
                gameOverMessage.text = $"Cricket Complete!\nFinal Score: {totalScore}";
            }
        }
    }

    private int CalculateScore(int hitArea)
    {
        if (hitArea >= 1 && hitArea <= 20)
            return hitArea;

        if (hitArea >= 21 && hitArea <= 38)
            return GetBaseNumber(hitArea) * 2;

        if (hitArea >= 39 && hitArea <= 58)
            return GetBaseNumber(hitArea) * 3;

        if (hitArea == 59) return 25;  // Outer bullseye
        if (hitArea == 60) return 50;  // Inner bullseye

        return 0;  // Miss
    }

    private int GetBaseNumber(int hitArea)
    {
        if (hitArea >= 1 && hitArea <= 20)
            return hitArea;

        if (hitArea >= 21 && hitArea <= 38)
            return boardSequence[hitArea - 21];

        if (hitArea >= 39 && hitArea <= 58)
            return boardSequence[hitArea - 39];

        return 0;
    }

    private IEnumerator DelayedNewTurn()
    {
        isTurnInProgress = true;
        yield return new WaitForSeconds(2f);
        StartNewTurn();
        isTurnInProgress = false;
    }

    private void StartNewTurn()
    {
        throwsLeft = 3;
        UpdateUI();
    }

    private void GameWon()
    {
        isGameOver = true;
        string message = currentGameMode == GameMode.Cricket ?
            $"Cricket Complete!\nFinal Score: {cricketNumbers.Sum(n => cricketScores[n])}" :
            "Congratulations! You Won!";
        ShowGameOverPanel(message);
    }

    private void GameOver(string message)
    {
        isGameOver = true;
        ShowGameOverPanel(message);
    }

    private void ShowGameOverPanel(string message)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverMessage != null)
            {
                gameOverMessage.text = message;
            }
        }
    }

    private void UpdateUI()
    {
        switch (currentGameMode)
        {
            case GameMode.Game501:
            case GameMode.Game301:
                scoreText.text = $"Score: {currentScore}";
                break;
            case GameMode.AroundTheClock:
                scoreText.text = $"Current Target: {currentScore}";
                break;
            case GameMode.Cricket:
                UpdateCricketUI();
                break;
        }
        throwsLeftText.text = $"Darts: {throwsLeft} (Total: {totalThrowsLeft})";
    }

    private void UpdateCricketUI()
    {
        string displayText = "Cricket Scores:\n";
        foreach (int number in cricketNumbers)
        {
            string marks = new string('/', cricketHits[number]);
            string closed = cricketHits[number] >= MARKS_TO_CLOSE ? " CLOSED" : "";
            displayText += $"{number}: {marks}{closed} - {cricketScores[number]} pts\n";
        }
        scoreText.text = displayText;
    }

    public void RestartGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        isGameOver = false;
        InitializeGame();
    }

    public void SetGameMode(GameMode newMode)
    {
        currentGameMode = newMode;
        InitializeGame();
    }
}