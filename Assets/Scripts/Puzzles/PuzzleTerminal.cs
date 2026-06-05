using UnityEngine;
using System.Collections.Generic;
public class PuzzleTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private MonoBehaviour puzzleUIBehaviour;
    public IPuzzle puzzleUI
    {
        get => puzzleUIBehaviour as IPuzzle;
        set => puzzleUIBehaviour = value as MonoBehaviour;
    }
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Player player;
    [SerializeField] private PuzzleRoom room;
    public RoomColors roomColor;

    private bool isOpen = false;
    private bool isSolved = false;


    public bool Interact()
    {
        Debug.Log("INTERACT");
        if (isOpen || isSolved)
            return false;

        OpenPuzzle();
        return true;
    }

    private void OpenPuzzle()
    {
        isOpen = true;

        PreparePuzzle(roomColor);
        puzzleUI.gameObject.SetActive(true);
        puzzleUI.OnPuzzleOpened();
        crosshair.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePuzzle()
    {
        isOpen = false;

        puzzleUI.gameObject.SetActive(false);
        crosshair.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.SetMovementEnabled();
    }

    public void FinishedPuzzle()
    {
        ClosePuzzle();
        isSolved = true;
        room.UnlockDoors();
    }

    public void PreparePuzzle(RoomColors roomColor)
    {
        Debug.Log("UFA");
        this.roomColor = roomColor;
        puzzleUI.SetTerminal(this);
        puzzleUI.Restart(roomColor);
    }
}