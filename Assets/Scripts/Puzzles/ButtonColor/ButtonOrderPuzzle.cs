using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ButtonOrderPuzzle : MonoBehaviour
{
    public string[] correctOrder = { "red", "blue", "green", "yellow"};

    private int currentIndex = 0;
    [SerializeField] PuzzleTerminal terminal;
    public void PressButton(Button clickedButton)
    {
        string buttonText = clickedButton.GetComponentInChildren<TMP_Text>().text;

        if (buttonText == correctOrder[currentIndex])
        {
            Debug.Log("Correct button!");

            currentIndex++;

            if (currentIndex >= correctOrder.Length)
            {
                PuzzleSolved();
            }
        }
        else
        {
            Debug.Log("Wrong button! Resetting puzzle.");
            currentIndex = 0;
        }
    }

    void PuzzleSolved()
    {
        Debug.Log("Puzzle Solved!");
        terminal.ClosePuzzle();
    }
}
