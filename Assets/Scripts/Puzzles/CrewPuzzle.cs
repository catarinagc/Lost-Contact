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
    public int valuesAmount = 8;
    private string[] femaleNames = { "Joana", "Carolina", "Mariana", "Maria", "Margarida", "Daniela", "Marta", "Beatriz", "Manuela", "Renata" };
    private string[] maleNames = { "Diogo", "Manuel", "Tiago", "Marco", "Daniel", "Ricardo", "Martim", "Emanuel"};
    private string[] times = { "01:20", "04:05", "06:30", "07:10", "10:25", "11:50", "12:00", "15:15", "16:40", "17:55", "18:30", "19:05", 
    "20:50", "21:20", "23:45", "14:00", "22:00", "03:45", "08:50", "05:00", "13:00", "02:40", "09:30"};
    private int[] codes = {2010, 6663, 9380, 3126, 7002, 0999, 5770, 5100, 6431, 8525, 8950, 1001, 2071, 
    0472, 2348, 8178, 4442, 0332, 1213, 0976, 2453, 8941, 5555};
    private int maleCount;
    private int femaleCount;
    List<string> shuffledTimes;
    List<int> shuffledCodes;
    private bool isReady = false;

    //Apenas aqui por causa da interface, nao preciso chamar
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

    public void CleanCode()
    {
        inputCode.text = "";
    }

    private void PrepareDropdown(List<string> finalNames)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(finalNames);
    }

    private List<string> CreatePairs()
    {
        maleCount = Random.Range(1, valuesAmount);
        femaleCount = valuesAmount - maleCount;

        List<string> maleList = new List<string>(maleNames);
        List<string> femaleList = new List<string>(femaleNames);
        List<string> finalNames = new List<string>();

        for (int i = 0; i < maleCount; i++)
        {
            int index = Random.Range(0, maleList.Count);
            finalNames.Add(maleList[index]);
            maleList.RemoveAt(index);
        }

        for (int i = 0; i < femaleCount; i++)
        {
            int index = Random.Range(0, femaleList.Count);
            finalNames.Add(femaleList[index]);
            femaleList.RemoveAt(index);
        }

        // Shuffle finalNames so males and females aren't grouped
        for (int i = finalNames.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (finalNames[i], finalNames[randomIndex]) = (finalNames[randomIndex], finalNames[i]);
        }

        shuffledTimes = new List<string>(times);
        shuffledCodes = new List<int>(codes);

        for (int i = shuffledTimes.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (shuffledTimes[i], shuffledTimes[randomIndex]) =
                (shuffledTimes[randomIndex], shuffledTimes[i]);
            (shuffledCodes[i], shuffledCodes[randomIndex]) =
                (shuffledCodes[randomIndex], shuffledCodes[i]); 
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < finalNames.Count; i++)
        {
            sb.AppendLine($"{finalNames[i]} - {shuffledTimes[i]}");
        }

        contentText.text = sb.ToString();
        return finalNames;
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

    // void Awake()
    // {
    //     PreparePuzzle();
    // }

    private string getEarliestRecord(List<string> finalNames)
    {
        int currentSmallestIndex = -1;
        int currentSmallestHour = 24;
        int currentSmallestMinuts = 59;
        for (int i = 0; i< valuesAmount; i++)
        {
            string[] parts = shuffledTimes[i].Split(':');
            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);
            if (hour < currentSmallestHour)
            {
                currentSmallestHour = hour;
                currentSmallestMinuts = minute;
                currentSmallestIndex = i;
            } else if (hour == currentSmallestHour && minute < currentSmallestMinuts)
            {
                currentSmallestHour = hour;
                currentSmallestMinuts = minute;
                currentSmallestIndex = i;
            }
        }
        return finalNames[currentSmallestIndex];
    }

    private string getLatestRecord(List<string> finalNames)
    {
        int currentSmallestIndex = -1;
        int currentSmallestHour = 0;
        int currentSmallestMinuts = 0;
        for (int i = 0; i< valuesAmount; i++)
        {
            string[] parts = shuffledTimes[i].Split(':');
            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);
            if (hour > currentSmallestHour)
            {
                currentSmallestHour = hour;
                currentSmallestMinuts = minute;
                currentSmallestIndex = i;
            } else if (hour == currentSmallestHour && minute > currentSmallestMinuts)
            {
                currentSmallestHour = hour;
                currentSmallestMinuts = minute;
                currentSmallestIndex = i;
            }
        }
        return finalNames[currentSmallestIndex];
    }

    private string getSecondLatestFemaleRecord(List<string> finalNames)
    {
        int latestIndex = 0;
        int secondLatestIndex = 0;
        int latestHour = 0, latestMinute = 0;
        int secondLatestHour = 0, secondLatestMinute = 0;

        for (int i = 0; i < valuesAmount; i++)
        {
            if (!IsFemale(finalNames[i])) continue;
            string[] parts = shuffledTimes[i].Split(':');
            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);

            if (hour > latestHour || (hour == latestHour && minute > latestMinute))
            {
                // Current latest becomes second latest
                secondLatestHour = latestHour;
                secondLatestMinute = latestMinute;
                secondLatestIndex = latestIndex;

                // Update latest
                latestHour = hour;
                latestMinute = minute;
                latestIndex = i;
            }
            else if (hour > secondLatestHour || (hour == secondLatestHour && minute > secondLatestMinute))
            {
                secondLatestHour = hour;
                secondLatestMinute = minute;
                secondLatestIndex = i;
            }
        }

        return finalNames[secondLatestIndex];
    }


    private string getThirdEarliestMaleRecord(List<string> finalNames)
    {
        int firstIndex = 0, secondIndex = 0, thirdIndex = 0;
        int firstHour = 24, firstMinute = 59;
        int secondHour = 24, secondMinute = 59;
        int thirdHour = 24, thirdMinute = 59;

        for (int i = 0; i < valuesAmount; i++)
        {
            if (IsFemale(finalNames[i])) continue;
            string[] parts = shuffledTimes[i].Split(':');
            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);

            if (hour < firstHour || (hour == firstHour && minute < firstMinute))
            {
                // Cascade down
                thirdHour = secondHour; thirdMinute = secondMinute; thirdIndex = secondIndex;
                secondHour = firstHour; secondMinute = firstMinute; secondIndex = firstIndex;

                firstHour = hour; firstMinute = minute; firstIndex = i;
            }
            else if (hour < secondHour || (hour == secondHour && minute < secondMinute))
            {
                thirdHour = secondHour; thirdMinute = secondMinute; thirdIndex = secondIndex;

                secondHour = hour; secondMinute = minute; secondIndex = i;
            }
            else if (hour < thirdHour || (hour == thirdHour && minute < thirdMinute))
            {
                thirdHour = hour; thirdMinute = minute; thirdIndex = i;
            }
        }

        return finalNames[thirdIndex];
    }

    private bool IsFemale(string name) => System.Array.IndexOf(femaleNames, name) >= 0;

    private void PrepareSolution(List<string> finalNames)
    {
        string name;
        int code;
        if (maleCount == femaleCount && getLatestRecord(finalNames).Length == 5)
        {
            name = getThirdEarliestMaleRecord(finalNames);
        } else if (maleCount > 3 && getEarliestRecord(finalNames).Length == 7)
        {
            name = getSecondLatestFemaleRecord(finalNames);
        } else if (femaleCount > 4 && getLatestRecord(finalNames).Length == 6)
        {
            name = getEarliestRecord(finalNames);
        } else
        {
            name = getEarliestRecord(finalNames);
        }
        correctPerson = name;
        int nameIndex = finalNames.IndexOf(name);
        correctCode = shuffledCodes[nameIndex];
    }

    private void PreparePuzzle()
    {
        List<string> finalNames = CreatePairs();
        PrepareDropdown(finalNames);
        PrepareSolution(finalNames);
        inputCode.text = "";
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
