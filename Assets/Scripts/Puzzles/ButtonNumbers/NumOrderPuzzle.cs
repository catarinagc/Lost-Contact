using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
public class NumOrderPuzzle : MonoBehaviour, IPuzzle
{
    //public int[] correctOrder = { 1, 9, 8, 7, 2 };
    public int[] correctOrder;
    [SerializeField] int codeLength = 5;
    private List<int> currentOrder = new List<int>();
    [SerializeField] TMP_Text symbolsDisplayText;
    [SerializeField] TMP_Text codeDisplayText;
    [SerializeField] GameObject wrongCodeText;
    [SerializeField] PuzzleTerminal terminal;
    private string[] greenCodes = { "ᛞ" , "ᛈ" , "ᚢ", "ᛤ", "ᛉ", "ᛗ", "ᚠ", "ᚡ", "ᛟ", "ᛦ"};
    private string[] blueCodes = { "ᚡ" , "ᛟ" , "ᛉ", "ᛞ", "ᛦ", "ᛗ", "ᛈ", "ᛤ", "ᚢ", "ᚠ"};
    public RoomColors roomColor;
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
        if (Input.GetKeyDown(KeyCode.Backspace) && currentOrder.Count > 0)
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

    void Awake()
    {
        PreparePuzzle();
    }

    private void PreparePuzzle()
    {
        codeDisplayText.text = "";
        correctOrder = PrepareCode();
        switch (roomColor)
        {
            case RoomColors.Blue:
                PrepareText(blueCodes);
                break;
            case RoomColors.Green:
                PrepareText(greenCodes);
                break;
            default:
                break;
        }
    }

    private int[] PrepareCode()
    {
        int[] code = new int[codeLength];
        for (int i = 0; i < codeLength; i++)
        {
            code[i] = Random.Range(0, 10);
        }
        return code;
    }

    private void PrepareText(string[] textCode)
    {
        symbolsDisplayText.text = "";
        foreach (int value in correctOrder)
        {
            symbolsDisplayText.text += textCode[value];
        }
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
}