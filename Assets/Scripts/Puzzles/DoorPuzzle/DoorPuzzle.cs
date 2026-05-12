using UnityEngine;

public class DoorPuzzle : MonoBehaviour
{
    [SerializeField] private Door[] doorList;
    [SerializeField] private Door correctDoor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (doorList != null)
        {
            foreach (Door door in doorList)
            {
                if (door != correctDoor)
                {
                    door.SetJammed(true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
