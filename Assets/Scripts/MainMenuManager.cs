using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // References to UI panels
    public GameObject mainMenuPanel;    // Panel containing main menu UI elements
    public GameObject gamePanel;        // Panel containing in-game UI elements
    public GameObject gameOverPanel;    // Panel for game over state

    // Reference to scoring system
    private DartScoring dartScoring;    // Handles game scoring logic

    // Initialize references and show menu on start
    void Start()
    {
        dartScoring = FindObjectOfType<DartScoring>();
        ShowMainMenu();
    }

    // Start 501 game mode
    public void StartGame501()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.Game501);
        StartGame();
    }

    // Start 301 game mode
    public void StartGame301()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.Game301);
        StartGame();
    }

    // Start Around the Clock game mode
    public void StartAroundTheClock()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.AroundTheClock);
        StartGame();
    }

    // Start Cricket game mode
    public void StartCricket()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.Cricket);
        StartGame();
    }

    // Start Math Game mode
    public void StartMathGame()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.MathGame);
        StartGame();
    }

    // Common game start logic
    private void StartGame()
    {
        mainMenuPanel.SetActive(false);    // Hide menu
        gamePanel.SetActive(true);         // Show game UI
        gameOverPanel.SetActive(false);    // Hide game over panel
        dartScoring.RestartGame();         // Reset game state
    }

    // Show main menu
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);     // Show menu
        gamePanel.SetActive(false);        // Hide game UI
        gameOverPanel.SetActive(false);    // Hide game over panel
    }

    // Show game over panel
    public void ShowGameOver()
    {
        mainMenuPanel.SetActive(false);    // Hide menu
        gamePanel.SetActive(false);        // Hide game UI
        gameOverPanel.SetActive(true);     // Show game over panel
    }

    // Restart current game
    public void RestartGame()
    {
        gameOverPanel.SetActive(false);    // Hide game over panel
        gamePanel.SetActive(true);         // Show game UI
        dartScoring.RestartGame();         // Reset game state
    }

    // Exit application
    public void QuitGame()
    {
        Application.Quit();                // Close the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}