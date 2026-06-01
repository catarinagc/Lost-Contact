using UnityEngine;

public class ExitDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private GameManager gameManager;

    private bool hasInteracted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Interact()
    {
        if (hasInteracted)
            return false;

        hasInteracted = true;

        //Debug.Log("Exit door interacted with.");

        if (gameManager != null)
        {
            gameManager.WinGame();
            return true;
        }

        //Debug.LogError("GameManager is not assigned on ExitDoor.");
        return false;
    }
}
