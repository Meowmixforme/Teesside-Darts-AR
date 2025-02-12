using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class DartScoring : MonoBehaviour
{
    // UI element references for displaying game information
    [Header("UI References")]
    public TMP_Text scoreText;            // Displays current score
    public TMP_Text throwsLeftText;       // Displays remaining throws
    public TMP_Text gameStateText;        // Displays game status messages
    [SerializeField] private GameObject gameOverPanel;     // Panel shown when game ends
    [SerializeField] private TMP_Text gameOverMessage;     // End game message
    [SerializeField] private Button restartButton;         // Button to restart game

    [Header("Audio")]
    private AudioSource hitSound;
    private AudioClip hitSoundClip;

    // Enum defining different game modes
    public enum GameMode
    {
        Game501,
        Game301,
        AroundTheClock,
        Cricket
    }

    // Game settings and configuration
    [Header("Game Settings")]
    [SerializeField] private GameMode currentGameMode = GameMode.Game501;
    private bool mustDoubleOut = true;    // Requirement for 501/301 games

    // Standard dartboard number sequence (clockwise)
    private readonly int[] boardSequence = new int[]
    {
        20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5
    };

    // Inner singles sequence
    private readonly int[] innerSingles = new int[]
    {
        1, 18, 4, 13, 6, 10, 15, 2, 3, 19, 7, 16, 8, 11, 14, 9, 12
    };

    // Game state tracking variables
    private int currentScore;             // Current game score
    private int throwsLeft = 3;           // Throws remaining in current turn
    private bool isGameOver = false;      // Tracks if game has ended
    private bool isDartboardSetup = false; // Tracks if dartboard is initialised
    private bool isTurnInProgress = false; // Tracks if turn is in progress

    // Cricket game specific variables
    private Dictionary<int, int> cricketScores = new Dictionary<int, int>();  // Scores for each number
    private Dictionary<int, int> cricketHits = new Dictionary<int, int>();    // Hits for each number
    private readonly int[] cricketNumbers = new int[] { 15, 16, 17, 18, 19, 20, 25 }; // Valid cricket numbers
    private const int MARKS_TO_CLOSE = 3;  // Hits needed to close a number

    // Maximum throws allowed for each game mode
    private const int MAX_THROWS_501 = 50;
    private const int MAX_THROWS_301 = 35;
    private const int MAX_THROWS_CLOCK = 30;
    private const int MAX_THROWS_CRICKET = 40;
    private int totalThrowsLeft;          // Total throws remaining in game

    // Initialise game state
    void Start()
    {
        // Load sound from Resources folder
        hitSoundClip = Resources.Load<AudioClip>("Sounds/SampleSound");

        // Create AudioSource component if it doesn't exist
        hitSound = gameObject.GetComponent<AudioSource>();
        if (hitSound == null)
        {
            hitSound = gameObject.AddComponent<AudioSource>();
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        InitialiseGame();
    }

    // Setup dartboard hit areas and tagging
    public void SetupDartboard(GameObject dartboard)
    {
        if (!isDartboardSetup)
        {
            Transform dartboardModel = dartboard.transform.Find("DartBoardModel");
            if (dartboardModel != null)
            {
                // Tag all hit areas
                for (int i = 1; i <= 79; i++)
                {
                    Transform hitArea = dartboardModel.Find($"HitArea.{i:D3}");
                    if (hitArea != null)
                    {
                        hitArea.gameObject.tag = "dart_board";
                    }
                }

                // Tag special rings
                Transform ring = dartboardModel.Find("Ring");
                Transform ring007 = dartboardModel.Find("Ring.007");
                Transform ring008 = dartboardModel.Find("Ring.008");

                if (ring != null) ring.gameObject.tag = "dart_board";
                if (ring007 != null) ring007.gameObject.tag = "dart_board";
                if (ring008 != null) ring008.gameObject.tag = "dart_board";
            }
            isDartboardSetup = true;
        }
    }

    // Initialise game based on selected mode
    private void InitialiseGame()
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
                InitialiseCricket();
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

    // Initialise cricket game variables
    private void InitialiseCricket()
    {
        cricketScores.Clear();
        cricketHits.Clear();
        foreach (int number in cricketNumbers)
        {
            cricketScores[number] = 0;
            cricketHits[number] = 0;
        }
    }

    // Process dart hit and score
    public void ScoreDart(Collider hitCollider)
    {
        // Return if game is over or no throws left
        if (isGameOver || throwsLeft <= 0) return;

        // Play hit sound
        if (hitSound != null && hitSoundClip != null)
        {
            hitSound.PlayOneShot(hitSoundClip);
        }

        bool scored = false;

        // Handle special rings
        if (hitCollider.gameObject.name == "Ring")
        {
            scored = ProcessSpecialRing(20); // Inner Single 20
        }
        else if (hitCollider.gameObject.name == "Ring.007")
        {
            scored = ProcessSpecialRing(25); // Outer Bull
        }
        else if (hitCollider.gameObject.name == "Ring.008")
        {
            scored = ProcessSpecialRing(50); // Inner Bull
        }
        else
        {
            // Validate hit area
            string hitAreaStr = hitCollider.gameObject.name;
            if (!hitAreaStr.StartsWith("HitArea.")) return;

            int hitArea;
            if (!int.TryParse(hitAreaStr.Substring(8), out hitArea)) return;

            // Score based on game mode
            switch (currentGameMode)
            {
                case GameMode.Game501:
                case GameMode.Game301:
                    scored = Score501Game(CalculateScore(hitArea));
                    break;
                case GameMode.AroundTheClock:
                    scored = ScoreAroundTheClock(GetBaseNumber(hitArea));
                    break;
                case GameMode.Cricket:
                    scored = ScoreCricket(GetBaseNumber(hitArea), GetMultiplier(hitArea));
                    break;
            }
        }

        // Update game state if scored
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

    // Process special ring hits (Inner 20, Bulls)
    private bool ProcessSpecialRing(int points)
    {
        switch (currentGameMode)
        {
            case GameMode.Game501:
            case GameMode.Game301:
                return Score501Game(points);
            case GameMode.AroundTheClock:
                if (points == 50 || points == 25) // Bulls
                    return ScoreAroundTheClock(25);
                return ScoreAroundTheClock(20); // Inner 20
            case GameMode.Cricket:
                if (points == 50) // Inner Bull
                    return ScoreCricket(25, 2);
                if (points == 25) // Outer Bull
                    return ScoreCricket(25, 1);
                return ScoreCricket(20, 1); // Inner 20
        }
        return false;
    }

    // Calculate score based on hit area
    private int CalculateScore(int hitArea)
    {
        // Inner Singles (1-17)
        if (hitArea >= 1 && hitArea <= 17)
            return innerSingles[hitArea - 1];

        // Inner Single 17
        if (hitArea == 79)
            return 17;

        // Outer Singles (19-38)
        if (hitArea >= 19 && hitArea <= 38)
            return boardSequence[hitArea - 19];

        // Triples (39-58)
        if (hitArea >= 39 && hitArea <= 58)
            return boardSequence[hitArea - 39] * 3;

        // Doubles (59-78)
        if (hitArea >= 59 && hitArea <= 78)
            return boardSequence[hitArea - 59] * 2;

        return 0;  // Miss or unhandled area
    }

    // Get base number from hit area
    private int GetBaseNumber(int hitArea)
    {
        // Inner Singles (1-17)
        if (hitArea >= 1 && hitArea <= 17)
            return innerSingles[hitArea - 1];

        // Inner Single 17
        if (hitArea == 79)
            return 17;

        // Outer Singles (19-38)
        if (hitArea >= 19 && hitArea <= 38)
            return boardSequence[hitArea - 19];

        // Triples (39-58)
        if (hitArea >= 39 && hitArea <= 58)
            return boardSequence[hitArea - 39];

        // Doubles (59-78)
        if (hitArea >= 59 && hitArea <= 78)
            return boardSequence[hitArea - 59];

        return 0;
    }

    // Get multiplier for hit area
    private int GetMultiplier(int hitArea)
    {
        if (hitArea >= 39 && hitArea <= 58) return 3;  // Triple
        if (hitArea >= 59 && hitArea <= 78) return 2;  // Double
        return 1;  // Single
    }

    // Score logic for 501/301 games
    private bool Score501Game(int points)
    {
        // Check if score would go below 0
        if (currentScore - points < 0)
        {
            if (gameStateText != null)
                gameStateText.text = "Bust!";
            return true;
        }

        // Check double-out rule
        if (mustDoubleOut && currentScore - points == 0)
        {
            bool isDouble = points == 50 || // Inner Bull
                           (points % 2 == 0 && points <= 40); // Regular double
            if (!isDouble)
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

    // Score logic for Around the Clock game
    private bool ScoreAroundTheClock(int baseNumber)
    {
        if (baseNumber == currentScore)
        {
            currentScore++;
            if (currentScore > 20)
            {
                GameWon();
            }
            return true;
        }
        return true;
    }

    // Score logic for Cricket game
    private bool ScoreCricket(int baseNumber, int multiplier)
    {
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

    // Check if cricket game is won
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

    // Delay before starting new turn
    private IEnumerator DelayedNewTurn()
    {
        isTurnInProgress = true;
        yield return new WaitForSeconds(2f);
        StartNewTurn();
        isTurnInProgress = false;
    }

    // Start new turn
    private void StartNewTurn()
    {
        throwsLeft = 3;
        UpdateUI();
    }

    // Handle game won state
    private void GameWon()
    {
        isGameOver = true;
        string message = currentGameMode == GameMode.Cricket ?
            $"Cricket Complete!\nFinal Score: {cricketNumbers.Sum(n => cricketScores[n])}" :
            "Congratulations! You Won!";
        ShowGameOverPanel(message);
    }

    // Handle game over state
    private void GameOver(string message)
    {
        isGameOver = true;
        ShowGameOverPanel(message);
    }

    // Display game over panel
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

    // Update UI elements
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

    // Update Cricket specific UI
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

    // Restart current game
    public void RestartGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        isGameOver = false;
        InitialiseGame();
    }

    // Change game mode
    public void SetGameMode(GameMode newMode)
    {
        currentGameMode = newMode;
        InitialiseGame();
    }
}