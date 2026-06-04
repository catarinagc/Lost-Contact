using UnityEngine;
using System.Collections.Generic;
public class PuzzleRoom : MonoBehaviour
{
    [SerializeField] List<Door> doorsToLock;
    [SerializeField] PuzzleTerminal terminal;
    public RoomColors roomColor;
    private bool isFirstTime = true;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entrou " + isFirstTime + " "+ other);
        if (other.CompareTag("Player") && isFirstTime)
        {
            Debug.Log("LOCK");
            terminal.PreparePuzzle(roomColor);
            isFirstTime = false;
            foreach( Door door in doorsToLock)
            {
                door.Lock();
            }
        }
    }

    public void UnlockDoors()
    {
        foreach( Door door in doorsToLock)
        {
            door.Unlock();
        }
    }
}
