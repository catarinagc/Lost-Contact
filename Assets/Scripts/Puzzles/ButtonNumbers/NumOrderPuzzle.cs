using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
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

    void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                currentOrder.Add(i);
                UpdateDisplay();
            }
            if (Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                currentOrder.Add(i);
                UpdateDisplay();
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            currentOrder.RemoveAt(currentOrder.Count - 1);
            UpdateDisplay();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitCode();
        }
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
        terminal.FinishedPuzzle();
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