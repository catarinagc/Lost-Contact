using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class WirePuzzle : MonoBehaviour, IPuzzle
{
    public string[] correctOrder;
    private string[] connected;
    private int currentIndex;
    private int currentConnected;
    private int currentCorrect;

    [SerializeField] GameObject wrongCodeText;
    [SerializeField] PuzzleTerminal terminal;
    [SerializeField] RectTransform wireParent;
    [SerializeField] Canvas canvas;
    public RoomColors roomColor;

    private Dictionary<int, RectTransform> activeWires = new Dictionary<int, RectTransform>();
    private RectTransform pendingWire;
    private RectTransform pendingFromButton;

    public void OnClick(Button clickedButton)
    {
        string buttonText = clickedButton.GetComponentInChildren<TMP_Text>().text.ToLower();

        if (int.TryParse(buttonText, out int parsedValue))
        {
            currentIndex = parsedValue - 1;
            Debug.Log($"Clicked number: {parsedValue}, index: {currentIndex}");
            Debug.Log($"connected[{currentIndex}] = '{connected[currentIndex]}'");
            Debug.Log($"connected length: {connected.Length}");
            Debug.Log($"currentConnected: {currentConnected}");
            if (connected[currentIndex] != "")
            {
                Debug.Log("HERE");
                connected[currentIndex] = "";
                currentConnected -= 1;

                // destroy the existing wire for this slot
                if (activeWires.ContainsKey(currentIndex))
                {
                    Destroy(activeWires[currentIndex].gameObject);
                    activeWires.Remove(currentIndex);
                }
            }

            // start drawing a pending wire from this number button
            pendingFromButton = clickedButton.GetComponent<RectTransform>();
            pendingWire = CreateWire();
        }
        else
        {
            buttonText = buttonText.ToUpper();

            if (currentIndex != -1 && pendingWire != null)
            {
                // snap the wire to the letter button
                RectTransform letterRect = clickedButton.GetComponent<RectTransform>();
                UpdateWire(pendingWire, pendingFromButton, GetAnchoredPos(letterRect));

                // store it
                activeWires[currentIndex] = pendingWire;
                pendingWire = null;
                pendingFromButton = null;

                connected[currentIndex] = buttonText;
                currentConnected += 1;

                if (currentConnected == correctOrder.Length)
                    SubmitCode();
            }
        }
        Debug.Log("VALUE "+ currentConnected);
    }

    void Update()
    {
        if (currentIndex != -1 && pendingWire != null && pendingFromButton != null)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                wireParent,
                Mouse.current.position.ReadValue(),
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out mousePos
            );

            UpdateWire(pendingWire, pendingFromButton, mousePos);
        }
    }

    // Creates a new wire image as a child of wireParent
    private RectTransform CreateWire()
    {
        GameObject wire = new GameObject("Wire", typeof(Image));
        wire.transform.SetParent(wireParent, false);

        Image img = wire.GetComponent<Image>();
        img.color = Color.black;

        RectTransform rt = wire.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0f, 0.5f);
        rt.sizeDelta = new Vector2(0f, 4f);

        return rt;
    }

    // Updates the wires position, size, and rotation to stretch from a RectTransform to a point
    private void UpdateWire(RectTransform wire, RectTransform from, Vector2 toPos)
    {
        Vector2 fromPos = GetAnchoredPos(from);

        Vector2 dir = toPos - fromPos;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        wire.anchoredPosition = fromPos;
        wire.sizeDelta = new Vector2(distance, 4f);
        wire.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    // Converts a buttons position into wireParents local space
    private Vector2 GetAnchoredPos(RectTransform rt)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            rt.position
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wireParent, screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        return localPoint;
    }

    public void SubmitCode()
    {
        bool isCorrect = true;
        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (correctOrder[i] != connected[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
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

    void CleanCode()
    {
        currentIndex = -1;
        currentCorrect = 0;
        currentConnected = 0;
        connected = new string[correctOrder.Length];
        for (int i = 0; i < connected.Length; i++)
            connected[i] = "";
        // destroy all active wires
        foreach (var wire in activeWires.Values)
            Destroy(wire.gameObject);
        activeWires.Clear();

        // destroy pending wire if any
        if (pendingWire != null)
        {
            Destroy(pendingWire.gameObject);
            pendingWire = null;
            pendingFromButton = null;
        }
    }

    IEnumerator ShowWrongCodeCoroutine()
    {
        wrongCodeText.SetActive(true);
        yield return new WaitForSeconds(2f);
        wrongCodeText.SetActive(false);
    }

    void Awake()
    {
        PreparePuzzle();
    }

    private void PreparePuzzle()
    {
        currentIndex = -1;
        currentCorrect = 0;
        currentConnected = 0;
        correctOrder = new[] { "A", "B", "C", "D", "E" };
        connected = new string[correctOrder.Length];
        for (int i = 0; i < connected.Length; i++)
            connected[i] = "";
        activeWires = new Dictionary<int, RectTransform>();
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