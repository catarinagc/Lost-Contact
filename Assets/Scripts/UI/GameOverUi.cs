using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject crosshair;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Scenes")]
    [SerializeField] private string levelSceneName = "Level";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Disable While Game Over")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private bool isOpen = false;

    public void ShowGameOver()
    {
        if (isOpen) return;

        isOpen = true;

        gameOverPanel.SetActive(true);

        if (crosshair != null)
            crosshair.SetActive(false);

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinueGame()
    {
        isOpen = false;

        gameOverPanel.SetActive(false);

        if (crosshair != null)
            crosshair.SetActive(true);

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (gameManager != null)
            gameManager.ContinueAfterGameOver();
    }

    public void Retry()
    {
        SceneManager.LoadScene(levelSceneName);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}