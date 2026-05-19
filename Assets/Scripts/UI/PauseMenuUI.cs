using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;

    [SerializeField] private MonoBehaviour[] scriptsToDisableWhenPaused;

    private bool isPaused = false;

    private bool[] previousScriptStates;
    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisible;

    private void Start()
    {
        previousScriptStates = new bool[scriptsToDisableWhenPaused.Length];

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
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
        if (isPaused) return;

        SaveStateBeforePause();

        isPaused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (MonoBehaviour script in scriptsToDisableWhenPaused)
        {
            if (script == null) continue;
            if (script == this) continue;

            script.enabled = false;
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisible;

        for (int i = 0; i < scriptsToDisableWhenPaused.Length; i++)
        {
            MonoBehaviour script = scriptsToDisableWhenPaused[i];

            if (script == null) continue;
            if (script == this) continue;

            script.enabled = previousScriptStates[i];
        }
    }

    public void ShowOptions()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        if (pausePanel != null)
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

    private void SaveStateBeforePause()
    {
        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        for (int i = 0; i < scriptsToDisableWhenPaused.Length; i++)
        {
            MonoBehaviour script = scriptsToDisableWhenPaused[i];

            if (script == null) continue;
            if (script == this) continue;

            previousScriptStates[i] = script.enabled;
        }
    }
}