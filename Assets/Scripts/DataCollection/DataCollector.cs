using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
    public Player player;
    public Vector3 lastPosition;
    public GameManager gameManager;

    public int explorerID = 0;
    //
    public string explorerPositionsFilePath = "";
    public string explorerPositionsFileName = "";
    public string dataFilePath = "";
    public string dataFileName = "";
    //
    public Vector3 explorerCurrentPosition = Vector3.zero;
    public float explorerDistanceTravelled = 0.001f;
    //
    public int minutesAlive = 0;
    public int secondsAlive = 0;
    //
    public bool gameOver = false;
    public bool explorerWon = false;
    //
    private bool writeInCSVFile = false;

    private void Awake()
    {
        if (ExplorerIDManager.Instance != null)
        {
            explorerID = ExplorerIDManager.Instance.explorerID;
        }
        else
        {
            explorerID = 999999;
        }
        //
        dataFilePath = Path.Combine(Application.persistentDataPath, "Data");
        dataFileName = "LOST_CONTACT_DATA.csv";
        CreateCSVFile(dataFilePath, dataFileName);
        //
        explorerPositionsFilePath = Path.Combine(Application.persistentDataPath, "Explorers", "EXPLORER-" + explorerID);
        explorerPositionsFileName = "EXPLORER-" + explorerID + "-HEATMAP-DATA.txt";
        CreateTxtFile(explorerPositionsFilePath, explorerPositionsFileName);
    }

    void Start()
    {
        if (player != null)
        {
            lastPosition = player.transform.position;
        }
        StartCoroutine(UpdateDistance());
        StartCoroutine(UpdateGameTime());
        StartCoroutine(WriteTXTData(explorerPositionsFilePath, explorerPositionsFileName));
    }

    // Update is called once per frame
    void Update()
    {
        if (writeInCSVFile)
        {
            if (gameManager.GetCurrentTime() <= 0.0f)
            {
                gameOver = true;
            }
            else if (gameManager.GetCurrentTime() >= 0.0f && gameManager.GetHasWon())
            {
                gameOver = true;
                explorerWon = true;
            }
            WriteCSVData(dataFilePath, dataFileName);
            writeInCSVFile = false;
        }

        //DEBUG COMMAND: TEST DATA EXTRACTION PURPOSES ONLY
        if (Input.GetKeyDown(KeyCode.F12))
        {
            gameManager.WinGame();
            writeInCSVFile = true;
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            gameManager.GameOver();
            writeInCSVFile = true;
        }
    }

    public IEnumerator UpdateGameTime()
    {
        while (!gameOver && !explorerWon)
        {
            yield return new WaitForSeconds(1f);

            secondsAlive++;

            if (secondsAlive >= 60)
            {
                secondsAlive = 0;
                minutesAlive++;
            }
        }
        Debug.LogWarning("GAME TIME UPDATED");
    }

    public IEnumerator UpdateDistance()
    {
        while (!gameOver && !explorerWon)
        {
            yield return new WaitForSeconds(1f);

            explorerCurrentPosition = player.transform.position;

            Vector3 delta = explorerCurrentPosition - lastPosition;
            delta.y = 0f;

            explorerDistanceTravelled += delta.magnitude;
            lastPosition = explorerCurrentPosition;
            Debug.LogWarning("DISTANCE UPDATED");
        }
    }

    public void WriteCSVData(string folderPath, string fileName) 
    {
        //File.WriteAllText(csvFilePath,
        //        "explorerID;" + "distanceTravelled;" +
        //        "explorerGameTime;" + "gameOver;" + "explorerWon;" + "\n"
        //        );

        string line =
            $"{explorerID};" + $"{explorerDistanceTravelled:F5};" +
            $"{minutesAlive};" + $"{secondsAlive};" + 
            $"{gameOver};" + $"{explorerWon}\n"; 

        string csvFilePath = Path.Combine(folderPath, fileName);
        File.AppendAllText(csvFilePath, line, Encoding.UTF8);

        //Debug.LogWarning("CSV DATA WRITTEN INTO FILE");
    }

    public IEnumerator WriteTXTData(string folderPath, string fileName)
    {
        string txtFilePath = Path.Combine(folderPath, fileName);

        while (!gameOver && !explorerWon)
        {
            yield return new WaitForSeconds(1f);

            string currentPositionString = $"{explorerCurrentPosition}\n";
            File.AppendAllText(txtFilePath, currentPositionString, Encoding.UTF8);
            //Debug.Log("POSITION DATA WRITTEN IN EXPLORER " + explorerID + " TXT FILE");
        }
    }

    //
    public void CreateCSVFile(string folderPath, string fileName)
    {
        Directory.CreateDirectory(folderPath);
        string csvFilePath = Path.Combine(folderPath, fileName);
        if (!File.Exists(csvFilePath))
        {
            File.WriteAllText(csvFilePath,
                "explorerID;" + "distanceTravelled;" + 
                "minutesAlive;" + "secondsAlive;" + 
                "gameOver;" + "explorerWon;" + "\n"
                );
        }
        Debug.Log("CSV File created at the following path: " + csvFilePath);
    }

    //
    public void CreateTxtFile(string folderPath, string fileName)
    {
        Directory.CreateDirectory(folderPath);

        string textFilePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(textFilePath))
        {
            File.WriteAllText(textFilePath,
                "");
        }

        //Debug.Log("Text file for explorer " + explorerID + "created at the following path: " + textFilePath);
    }
}
