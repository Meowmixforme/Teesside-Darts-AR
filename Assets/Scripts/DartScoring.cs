using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

// Main scoring system for AR Darts. Handles scoring, game modes and UI
public class DartScoring : MonoBehaviour
{
    // UI elements
    [Header("UI References")]
    public TMP_Text scoreText;            // Shows current score
    public TMP_Text throwsLeftText;       // Shows remaining throws
    public TMP_Text gameStateText;        // Shows game messages
    [SerializeField] private GameObject gameOverPanel;     // End game panel
    [SerializeField] private TMP_Text gameOverMessage;     // End game text
    [SerializeField] private Button restartButton;         // Restart button

    // Sound settings
    [Header("Audio")]
    private AudioSource hitSound;         // Sound source
    private AudioClip hitSoundClip;       // Hit sound effect

    // Available game modes
    public enum GameMode
    {
        Game501,
        Game301,
        AroundTheClock,
        Cricket,
        MathGame
    }

    [Header("Game Settings")]
    [SerializeField] private GameMode currentGameMode = GameMode.Game501;

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

    // Game state tracking
    private bool isGameOver = false;
    private bool isDartboardSetup = false;
    private bool isTurnInProgress = false;

    // Current game mode instance
    private IDartGameMode currentGame;

    // Set up the game when starting
    void Start()
    {
        // Load hit sound
        hitSoundClip = Resources.Load<AudioClip>("Sounds/SampleSound");

        // Set up sound source
        hitSound = gameObject.GetComponent<AudioSource>();
        if (hitSound == null)
        {
            hitSound = gameObject.AddComponent<AudioSource>();
        }

        // Hide game over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        InitialiseGame();
    }

    // Set up dartboard hit areas and tags
    public void SetupDartboard(GameObject dartboard)
    {
        if (!isDartboardSetup)
        {
            Transform dartboardModel = dartboard.transform.Find("DartBoardModel");
            if (dartboardModel != null)
            {
                // Tag all scoring areas
                for (int i = 1; i <= 79; i++)
                {
                    Transform hitArea = dartboardModel.Find($"HitArea.{i:D3}");
                    if (hitArea != null)
                    {
                        hitArea.gameObject.tag = "dart_board";
                    }
                }

                // Tag special rings (singles and bulls)
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

    // Start new game with current mode
    private void InitialiseGame()
    {
        try
        {
            // Create game mode instance
            currentGame = currentGameMode switch
            {
                GameMode.Game501 => new Game501(),
                GameMode.Game301 => new Game301(),
                GameMode.AroundTheClock => new AroundTheClockGame(),
                GameMode.Cricket => new CricketGame(),
                GameMode.MathGame => new MathGame(),
                _ => new Game501() // Default to 501
            };

            currentGame.InitializeGame();
            isGameOver = false;
            isTurnInProgress = false;

            // Reset UI
            if (gameStateText != null)
                gameStateText.text = "";
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            UpdateUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Game initialisation error: {e.Message}");
        }
    }

    // Handle dart hits and scoring
    public void ScoreDart(Collider hitCollider)
    {
        if (isGameOver || currentGame.GetThrowsLeft() <= 0) return;

        // Play hit sound
        if (hitSound != null && hitSoundClip != null)
        {
            hitSound.PlayOneShot(hitSoundClip);
        }

        bool scored = false;
        int baseNumber;
        int multiplier;

        // Handle special rings
        if (hitCollider.gameObject.name == "Ring")
        {
            baseNumber = 20;
            multiplier = 1;
            scored = true;
        }
        else if (hitCollider.gameObject.name == "Ring.007")
        {
            baseNumber = 25;
            multiplier = 1;
            scored = true;
        }
        else if (hitCollider.gameObject.name == "Ring.008")
        {
            baseNumber = 25;
            multiplier = 2;
            scored = true;
        }
        else
        {
            // Validate hit area
            string hitAreaStr = hitCollider.gameObject.name;
            if (!hitAreaStr.StartsWith("HitArea.")) return;

            int hitArea;
            if (!int.TryParse(hitAreaStr.Substring(8), out hitArea)) return;

            baseNumber = GetBaseNumber(hitArea);
            multiplier = GetMultiplier(hitArea);
            scored = true;
        }

        if (scored)
        {
            bool validScore = currentGame.ProcessScore(baseNumber, multiplier);

            if (validScore)
            {
                if (currentGame.CheckWinCondition())
                {
                    GameWon();
                }
                else if (currentGame.GetThrowsLeft() <= 0)
                {
                    StartCoroutine(DelayedNewTurn());
                }

                // Check for total throws limit
                if (currentGame.GetTotalThrowsLeft() <= 0 && !isGameOver)
                {
                    GameOver("Out of throws! Game Over!");
                    return;
                }

                // Handle specific game mode messages
                if (currentGameMode == GameMode.MathGame)
                {
                    if (gameStateText != null)
                        gameStateText.text = "Correct!";
                }
            }
            else
            {
                // Handle invalid scores and display appropriate messages
                if (currentGameMode == GameMode.Game501 || currentGameMode == GameMode.Game301)
                {
                    if (currentGame.IsDoubleOutRequired() && currentGame.WouldFinishGame(baseNumber * multiplier))
                    {
                        if (gameStateText != null)
                            gameStateText.text = "Must finish on a double!";
                    }
                    else
                    {
                        if (gameStateText != null)
                            gameStateText.text = "Bust!";
                    }
                }
                else if (currentGameMode == GameMode.MathGame)
                {
                    if (gameStateText != null)
                    {
                        MathGame mathGame = currentGame as MathGame;
                        if (mathGame != null)
                        {
                            gameStateText.text = $"Wrong! The correct number was {mathGame.GetCurrentAnswer()}";
                        }
                        else
                        {
                            gameStateText.text = "Wrong answer!";
                        }
                    }
                }
            }

            UpdateUI();

            // Start new turn if out of throws
            if (currentGame.GetThrowsLeft() <= 0 && !isGameOver)
            {
                StartCoroutine(DelayedNewTurn());
            }
        }
    }

    // Get base number from hit area
    private int GetBaseNumber(int hitArea)
    {
        // Inner singles (1-17)
        if (hitArea >= 1 && hitArea <= 17)
            return innerSingles[hitArea - 1];

        // Inner single 17
        if (hitArea == 79)
            return 17;

        // Outer singles (19-38)
        if (hitArea >= 19 && hitArea <= 38)
            return boardSequence[hitArea - 19];

        // Triples (39-58)
        if (hitArea >= 39 && hitArea <= 58)
            return boardSequence[hitArea - 39];

        // Doubles (59-78)
        if (hitArea >= 59 && hitArea <= 78)
            return boardSequence[hitArea - 59];

        return 0;  // Miss or invalid area
    }

    // Get score multiplier from hit area
    private int GetMultiplier(int hitArea)
    {
        if (hitArea >= 39 && hitArea <= 58) return 3;  // Triple
        if (hitArea >= 59 && hitArea <= 78) return 2;  // Double
        return 1;  // Single
    }

    // Calculate total score for hit area
    private int CalculateScore(int hitArea)
    {
        return GetBaseNumber(hitArea) * GetMultiplier(hitArea);
    }

    // Wait before starting new turn
    private IEnumerator DelayedNewTurn()
    {
        isTurnInProgress = true;
        if (gameStateText != null)
            gameStateText.text = "New turn starting...";

        yield return new WaitForSeconds(2f);

        currentGame.StartNewTurn();
        isTurnInProgress = false;

        if (gameStateText != null)
            gameStateText.text = "";

        UpdateUI();
    }

    // Handle game won state
    private void GameWon()
    {
        isGameOver = true;
        string message = currentGameMode == GameMode.Cricket ?
            $"Cricket Complete!\nFinal Score: {(currentGame as CricketGame)?.GetTotalScore()}" :
            "Congratulations! You Won!";
        ShowGameOverPanel(message);
    }

    // Handle game over state
    private void GameOver(string message)
    {
        isGameOver = true;
        ShowGameOverPanel(message);
    }

    // Show game over panel with message
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

    // Update all UI elements
    private void UpdateUI()
    {
        try
        {
            if (scoreText != null)
                scoreText.text = currentGame.GetScoreDisplay();

            if (throwsLeftText != null)
                throwsLeftText.text = $"Darts: {currentGame.GetThrowsLeft()} (Total: {currentGame.GetTotalThrowsLeft()})";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UI update error: {e.Message}");
        }
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

    // Change to new game mode
    public void SetGameMode(GameMode newMode)
    {
        currentGameMode = newMode;
        InitialiseGame();
    }
}