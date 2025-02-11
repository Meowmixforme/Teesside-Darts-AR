using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject gamePanel;

    private DartScoring dartScoring;

    void Start()
    {
        dartScoring = FindObjectOfType<DartScoring>();
        ShowMainMenu();
    }

    public void StartGame501()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.Game501);
        StartGame();
    }

    public void StartGame301()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.Game301);
        StartGame();
    }

    public void StartAroundTheClock()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.AroundTheClock);
        StartGame();
    }

    public void StartCricket()
    {
        dartScoring.SetGameMode(DartScoring.GameMode.Cricket);
        StartGame();
    }

    private void StartGame()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(true);
        dartScoring.RestartGame();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}