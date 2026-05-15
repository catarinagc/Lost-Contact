using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string levelSceneName = "Level";

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        ShowMainMenu();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        // LoadSceneMode.Single evita que o Main Menu fique carregado por cima do Level.
        SceneManager.LoadScene(levelSceneName, LoadSceneMode.Single);
    }

    public void ShowMainMenu()
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ShowControls()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}