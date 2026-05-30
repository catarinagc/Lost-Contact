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

        Debug.LogWarning("TIMER STARTED! GET OUT!");
    }

    void Update()
    {
        if (timerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.unscaledDeltaTime;

                if (currentTime <= 0)
                {
                    currentTime = 0;
                    GameOver();
                }

                UpdateTimerDisplay(currentTime);
            }
        }

        if (reverseTimer)
        {
            currentTime += Time.unscaledDeltaTime;
            UpdateTimerDisplay(currentTime);
        }
    }

    void GameOver()
    {
        if (gameOverShown) return;

        gameOverShown = true;
        timerRunning = false;
        reverseTimer = true;

        timerText.text = "GAME OVER\n00:00";
        timerText.color = Color.red;

        currentTime = 0;
    }

    public void WinGame()
    {
        if (hasWon || gameOverShown) return;

        hasWon = true;

        reverseTimer = false;
        timerRunning = false;

        timerText.color = Color.green;
        timerText.text = timerText.text + "\nYOU WON!";

        StartCoroutine(LoadEndingCutsceneAfterDelay());
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

    // private void PreparePuzzles()
    // {
    //     foreach (PuzzleTerminal terminal in puzzleTerminals)
    //     {
    //         int randPuzzle = Random.Range(0, puzzles.Count);
    //         terminal.puzzleUI = puzzles[randPuzzle];
    //     }
    // }

    // GameManager.cs
    private void PreparePuzzles()
    {
        foreach (var mb in puzzleObjects)
        {
            Debug.Log($"Type: {mb.GetType().FullName}, Interfaces: {string.Join(", ", mb.GetType().GetInterfaces().Select(i => i.FullName))}");
        }

        puzzles = puzzleObjects.Cast<IPuzzle>().ToList();
        List<IPuzzle> shuffled = puzzles.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < puzzleTerminals.Count; i++)
        {
            IPuzzle assigned = shuffled[i % shuffled.Count];
            puzzleTerminals[i].puzzleUI = assigned;
            assigned.gameObject.SetActive(false);
        }
    }

}