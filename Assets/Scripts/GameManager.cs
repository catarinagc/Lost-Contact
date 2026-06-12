using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] int totalMinutes = 10;
    [SerializeField] int totalSeconds = 30;

    [Header("Ending")]
    [SerializeField] private string endingCutsceneSceneName = "EndingCutscene";
    [SerializeField] private float delayBeforeEndingCutscene = 2f;

    [Header("Puzzles")]
    [SerializeField] private List<MonoBehaviour> puzzleObjects;
    private List<IPuzzle> puzzles;
    [SerializeField] List<PuzzleTerminal> puzzleTerminals;

    [Header("Game Over")]
    [SerializeField] private GameOverUI gameOverUI;
    private bool gameOverScreenOpen = false;

    private float currentTime;
    private bool timerRunning = true;
    private bool reverseTimer = false;
    private bool gameOverShown = false;
    private bool hasWon = false;

    private Color originalColor;

    void Start()
    {
        PreparePuzzles();
        currentTime = totalMinutes * 60 + totalSeconds;
        originalColor = timerText.color;

        //Debug.LogWarning("TIMER STARTED! GET OUT!");
    }

    void Update()
    {
        if (timerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.unscaledDeltaTime;
            }else
            {
                currentTime = 0;
                GameOver();
            }

                UpdateTimerDisplay(currentTime);
        }

        if (reverseTimer)
        {
            currentTime += Time.unscaledDeltaTime;
            UpdateTimerDisplay(currentTime);
        }
    }

    //

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public bool GetHasWon()
    {
        return hasWon;
    }

    public bool GetGameOverShown()
    {
        return gameOverShown;
    }

    //

    public void GameOver()
    {
        if (gameOverShown) return;

        gameOverShown = true;
        gameOverScreenOpen = true;

        timerRunning = false;
        reverseTimer = false;

        currentTime = 0;

        // Important: close any puzzle before showing Game Over UI
        CloseAllPuzzles();

        timerText.text = "GAME OVER\n00:00";
        timerText.color = Color.red;

        if (gameOverUI != null)
            gameOverUI.ShowGameOver();
    }


    public void WinGame()
    {
        if (hasWon || gameOverScreenOpen) return;

        hasWon = true;

        reverseTimer = false;
        timerRunning = false;

        timerText.color = Color.green;
        timerText.text = timerText.text + "\nYOU WON!";

        StartCoroutine(LoadEndingCutsceneAfterDelay());
    }

    public void ContinueAfterGameOver()
    {
        if (!gameOverShown || hasWon) return;

        gameOverScreenOpen = false;

        timerRunning = false;
        reverseTimer = true;

        currentTime = 0;

        timerText.color = Color.red;
        UpdateTimerDisplay(currentTime);
    }

    private IEnumerator LoadEndingCutsceneAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delayBeforeEndingCutscene);

        SceneManager.LoadScene(endingCutsceneSceneName, LoadSceneMode.Single);
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Only overwrite if not showing GAME OVER text
        if (!gameOverShown)
            timerText.text = timeString;
        else
            timerText.text = "GAME OVER\n" + timeString;
    }

    public void ErrorPenalty()
    {
        if (hasWon) return;

        if (timerRunning)
        {
            currentTime -= 10f;
            StartCoroutine(FlashRed());
        }
        else
        {
            currentTime += 10f;
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        timerText.color = Color.red;
        yield return new WaitForSecondsRealtime(0.2f);
        timerText.color = gameOverShown ? Color.red : originalColor;
    }

    // GameManager.cs
    private void PreparePuzzles()
    {
        puzzles = puzzleObjects.Cast<IPuzzle>().ToList();
        List<IPuzzle> shuffled = puzzles.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < puzzleTerminals.Count; i++)
        {
            IPuzzle assigned = shuffled[i % shuffled.Count];
            puzzleTerminals[i].puzzleUI = assigned;
            assigned.gameObject.SetActive(false);
        }
    }

    private void CloseAllPuzzles()
    {
        foreach (PuzzleTerminal terminal in puzzleTerminals)
        {
            if (terminal != null)
                terminal.ClosePuzzle();
        }
    }

}