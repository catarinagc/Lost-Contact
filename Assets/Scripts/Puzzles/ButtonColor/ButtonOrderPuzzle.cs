using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonOrderPuzzle : MonoBehaviour
{
    public string[] correctOrder = { "red", "blue", "green", "yellow" };

    private int currentIndex = 0;
    private int pressedIndex = 0;

    [SerializeField] PuzzleTerminal terminal;
    [SerializeField] GameObject wrongText;

    [Header("Puzzle SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip solvedSound;
    [SerializeField] private float sfxVolume = 0.8f;

    private Coroutine currentTextRoutine;

    public void PressButton(Button clickedButton)
    {
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

        // Reset puzzle state
        currentIndex = 0;
        pressedIndex = 0;

        // Re-enable all buttons
        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    void PuzzleSolved()
    {
        Debug.Log("Puzzle Solved!");
        PlaySFX(solvedSound);
        terminal.FinishedPuzzle();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, sfxVolume);
    }
}