using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonOrderPuzzle : MonoBehaviour, IPuzzle
{
    public string[] correctOrder;
    private int currentIndex = 0;
    private int pressedIndex = 0;

    [SerializeField] public PuzzleTerminal terminal;
    [SerializeField] GameObject wrongText;

    [Header("Puzzle SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip solvedSound;
    [SerializeField] private float sfxVolume = 0.8f;
    public RoomColors roomColor;
    private Coroutine currentTextRoutine;

    private bool isReady = false;

    public void OnPuzzleOpened()
    {
        isReady = false;
        StartCoroutine(EnableAfterFrame());
    }

    private IEnumerator EnableAfterFrame()
    {
        // Wait 2 frames to let the opening click fully pass
        yield return null;
        yield return null;
        isReady = true;
    }

    // void Awake()
    // {
    //     PreparePuzzle();
    // }

    private void PreparePuzzle()
    {
        CleanCode();
        currentIndex = 0;
        pressedIndex = 0;
        foreach(Button button in gameObject.GetComponentsInChildren<Button>())
        {
            button.interactable = true;
        }
        switch (roomColor)
        {
            case RoomColors.Blue:
                correctOrder = new[] { "blue", "yellow", "red", "green" };
                break;
            case RoomColors.Green:
                correctOrder = new[] { "red", "blue", "green", "yellow" };
                break;
            default:
                break;
        }
    }

    public void PressButton(Button clickedButton)
    {
        if (!isReady) return;
        // Ignore already pressed buttons
        if (!clickedButton.interactable)
            return;

        string buttonText = clickedButton.GetComponentInChildren<TMP_Text>().text.ToLower();

        // Keep button visually pressed / disabled
        clickedButton.interactable = false;

        if (buttonText == correctOrder[currentIndex])
        {
            currentIndex++;
            pressedIndex++;
        }
        else
        {
            currentIndex = 0;
            pressedIndex++;
        }

        if (pressedIndex == correctOrder.Length)
        {
            if (currentIndex >= correctOrder.Length)
            {
                PuzzleSolved();
            }
            else
            {
                if (currentTextRoutine != null)
                {
                    StopCoroutine(currentTextRoutine);
                }

                currentTextRoutine = StartCoroutine(ResetPuzzle());
            }
        }
    }

    private IEnumerator ResetPuzzle()
    {
        wrongText.SetActive(true);
        PlaySFX(wrongSound);

        yield return new WaitForSeconds(2f);

        wrongText.SetActive(false);

        CleanCode();
    }

    void PuzzleSolved()
    {
        PlaySFX(solvedSound);
        terminal.FinishedPuzzle();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, sfxVolume);
    }

    public void Restart(RoomColors roomColor)
    {
        this.roomColor = roomColor;
        PreparePuzzle();
    }

    public void ClosePuzzle()
    {
        terminal.ClosePuzzle();
    }

    public void SetTerminal(PuzzleTerminal terminal)
    {
        this.terminal = terminal;
    }

    private void CleanCode()
    {
        currentIndex = 0;
        pressedIndex = 0;

        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }
}