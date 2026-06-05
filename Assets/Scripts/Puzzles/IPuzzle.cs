using UnityEngine;

public interface IPuzzle
{
    GameObject gameObject { get; }
    void Restart(RoomColors roomColor);
    void ClosePuzzle();
    void SetTerminal(PuzzleTerminal terminal);
    void OnPuzzleOpened();
}
