using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;

    [SerializeField] private MonoBehaviour[] scriptsToDisableWhenPaused;

    private bool isPaused = false;

    private void Start()
    {
        SetPaused(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (optionsPanel != null && optionsPanel.activeSelf)
                    ShowPauseMenu();
                else
                    ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        SetPaused(true);
        ShowPauseMenu();
    }

    public void ResumeGame()
    {
        SetPaused(false);
    }

    public void ShowOptions()
    {
        pausePanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        pausePanel.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetPaused(bool pause)
    {
        isPaused = pause;

        pausePanel.SetActive(pause);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        
        if (pause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        foreach (MonoBehaviour script in scriptsToDisableWhenPaused)
        {
            if (script == null) continue;
            if (script == this) continue;

            script.enabled = !pause;
        }
    }
}