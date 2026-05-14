using UnityEngine;
using System.Collections.Generic;
public class PuzzleRoom : MonoBehaviour
{
    [SerializeField] List<Door> doorsToLock;
    private bool isFirstTime = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isFirstTime)
        {
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
