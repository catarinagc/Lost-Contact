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
    bool isFirstTime = true;

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

    public void OnClick()
    {
        if (isFirstTime)
        {
            isFirstTime = false;
            return;
        }
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
        currentValue = startValue;
        UpdateScreen();
    }

    private void PreparePuzzle()
    {
        CleanCode();
        startValue = Random.Range(0, 20);
        currentValue = startValue;
        switch(roomColor)
        {
            case RoomColors.Blue:
                correctValue = startValue + 8;
                break;
            case RoomColors.Green:
                correctValue = startValue + 10;
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
        isFirstTime = true;
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
