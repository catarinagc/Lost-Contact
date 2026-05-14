using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class NumOrderPuzzle : MonoBehaviour
{
    public int[] correctOrder = { 1, 9, 8, 7, 2 };

    private List<int> currentOrder = new List<int>();

    [SerializeField] TMP_Text codeDisplayText;
    [SerializeField] GameObject wrongCodeText;
    [SerializeField] PuzzleTerminal terminal;

    public void PressButton(Button clickedButton)
    {
        string buttonText = clickedButton.GetComponentInChildren<TMP_Text>().text;

        int digit = int.Parse(buttonText);

        currentOrder.Add(digit);

        UpdateDisplay();
    }

    public void CleanCode()
    {
        currentOrder.Clear();

        UpdateDisplay();
    }

    public void SubmitCode()
    {
        if (currentOrder.Count != correctOrder.Length)
        {
            WrongCode();
            return;
        }

        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (correctOrder[i] != currentOrder[i])
            {
                WrongCode();
                return;
            }
        }

        PuzzleSolved();
    }

    void WrongCode()
    {
        Debug.Log("Wrong code!");

        StartCoroutine(ShowWrongCode());
        
        CleanCode();
    }

    IEnumerator ShowWrongCode()
    {
        wrongCodeText.SetActive(true);

        yield return new WaitForSeconds(2f);

        wrongCodeText.SetActive(false);
    }

    void PuzzleSolved()
    {
        Debug.Log("Puzzle Solved!");
        terminal.ClosePuzzle();
    }

    void UpdateDisplay()
    {
        codeDisplayText.text = "";

        foreach (int digit in currentOrder)
        {
            codeDisplayText.text += digit.ToString();
        }
    }
}