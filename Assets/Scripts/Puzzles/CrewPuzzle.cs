using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using System.Text;
public class CrewPuzzle : MonoBehaviour, IPuzzle
{
    [SerializeField] PuzzleTerminal terminal;
    public RoomColors roomColor;
    [SerializeField] GameObject wrongCodeText;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] GameObject content;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] TMP_InputField inputCode;
    [SerializeField] int correctCode;
    [SerializeField] string correctPerson;
    private string[] names = { "Joana", "Diogo", "Manuel", "Carolina", "Tiago", "Marco", "Mariana", "Maria" };
    private string[] times = { "22:00", "13:00", "5:00", "3:45", "9:30", "8:50", "14:00", "2:40"};

    public void CleanCode()
    {
        inputCode.text = "";
    }

    private void PrepareDropdown()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(names));
    }

    private void CreatePairs()
    {
        List<string> shuffledTimes = new List<string>(times);

        for (int i = shuffledTimes.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (shuffledTimes[i], shuffledTimes[randomIndex]) =
                (shuffledTimes[randomIndex], shuffledTimes[i]);
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < names.Length; i++)
        {
            sb.AppendLine($"{names[i]} - {shuffledTimes[i]}");
        }

        contentText.text = sb.ToString();
    }

    public void SubmitCode()
    {
        string cleanInput = inputCode.text
            .Trim()
            .Replace("\u200B", ""); // remove zero-width space TMP injects

        if (!int.TryParse(cleanInput, out int enteredCode))
        {
            WrongCode();
            return;
        }

        if (correctCode == enteredCode && correctPerson == dropdown.options[dropdown.value].text)
            PuzzleSolved();
        else
        {
            Debug.Log($"Wrong: entered {enteredCode} vs correct {correctCode}");
            WrongCode();
        }
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

    void Awake()
    {
        PreparePuzzle();
    }

    private void PreparePuzzle()
    {
        correctCode = 5555;
        correctPerson = "Maria";
        inputCode.text = "";
        PrepareDropdown();
        CreatePairs();
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
