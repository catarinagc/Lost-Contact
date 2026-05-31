using UnityEngine;

public interface IPuzzle
{
    GameObject gameObject { get; }
    void Restart(RoomColors roomColor);
    void ClosePuzzle();
}
