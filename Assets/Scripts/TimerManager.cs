using UnityEngine;
using TMPro;
public class TimerManager : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] int totalMinutes = 1;
    [SerializeField] int totalSeconds = 0;
    private float currentTime;
    private bool timerRunning = true;
    private bool reverseTimer = false;

    void Start()
    {
        currentTime = totalMinutes * 60 + totalSeconds;
    }

    void Update()
    {
        if (timerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;

                if (currentTime < 0)
                    currentTime = 0;

                UpdateTimerDisplay(currentTime);
            }
            else
            {
                timerRunning = false;
                Debug.Log("Time's up!");
                ReverseTimer();
            }
        }
        if (reverseTimer)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay(currentTime);
        }
    }

    void ReverseTimer()
    {
        reverseTimer = true;
        currentTime = 0;
        timerText.color = Color.red;
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}