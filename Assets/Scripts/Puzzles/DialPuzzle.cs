using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class DialPuzzle : MonoBehaviour, IPuzzle
{
    public RoomColors roomColor;
    [SerializeField] GameObject wrongCodeText;
    [SerializeField] PuzzleTerminal terminal;
    [SerializeField] TMP_Text valueText;
    private int currentValue;
    [SerializeField] int correctValue;

    void Awake()
    {
        PreparePuzzle();
    }

    public void Up()
    {
        currentValue += 1;
        if (currentValue == 10)
            currentValue = 0;
        UpdateScreen();
    }

    public void Down()
    {
        currentValue -= 1;
        if (currentValue == -1)
            currentValue = 9;
        UpdateScreen();
    }

    private void UpdateScreen()
    {
        valueText.text = currentValue.ToString();
    }

    public void SubmitCode()
    {
        if (currentValue == correctValue)
            PuzzleSolved();
        else
            WrongCode();
    }

    void PuzzleSolved()
    {
        terminal.FinishedPuzzle();
    }

    void WrongCode()
    {
        StartCoroutine(ShowWrongCodeCoroutine());
        CleanCode();
    }
    IEnumerator ShowWrongCodeCoroutine()
    {
        wrongCodeText.SetActive(true);
        yield return new WaitForSeconds(2f);
        wrongCodeText.SetActive(false);
    }

    void CleanCode()
    {
        currentValue = 0;
        UpdateScreen();
    }

    private void PreparePuzzle()
    {
        correctValue = 8;
        currentValue = 0;
        UpdateScreen();
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
