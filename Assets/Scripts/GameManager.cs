using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] int totalMinutes = 10;
    [SerializeField] int totalSeconds = 30;

    private float currentTime;
    private bool timerRunning = true;
    private bool reverseTimer = false;
    private bool gameOverShown = false;

    private Color originalColor;

    void Start()
    {
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
        reverseTimer = false;
        timerRunning = false;

        timerText.color = Color.green;
        timerText.text = timerText.text + "\nYOU WON!";
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
}