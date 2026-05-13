using UnityEngine;

public class PuzzleTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject puzzleUI;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Player player;
    private bool isOpen = false;

    public void Interact()
    {
        if (isOpen)
            return;

        OpenPuzzle();
    }

    private void OpenPuzzle()
    {
        isOpen = true;

        puzzleUI.SetActive(true);
        crosshair.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePuzzle()
    {
        isOpen = false;

        puzzleUI.SetActive(false);
        crosshair.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.SetMovementEnabled();
    }
}