using UnityEngine;
using System.Collections.Generic;
public class PuzzleRoom : MonoBehaviour
{
    [SerializeField] List<Door> doorsToLock;
    [SerializeField] PuzzleTerminal terminal;
    private bool isFirstTime = true;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entrou " + isFirstTime + " "+ other);
        if (other.CompareTag("Player") && isFirstTime)
        {
            terminal.PreparePuzzle();
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
