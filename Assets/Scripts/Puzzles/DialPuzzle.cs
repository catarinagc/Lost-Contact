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
    private int startValue;
    private int currentValue;
    [SerializeField] int correctValue;

    void Awake()
    {
        PreparePuzzle();
    }

    public void OnClick()
    {
        currentValue += 1;
        UpdateScreen();
    }

    public void RestartShownValue()
    {
        currentValue = startValue;
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
        correctValue = Random.Range(0, 20);
        currentValue = 0;
        startValue = correctValue;
        switch(roomColor)
        {
            case RoomColors.Blue:
                correctValue = currentValue + 8;
                break;
            case RoomColors.Green:
                correctValue = currentValue + 10;
                break;
            default:
                break;
        }
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
