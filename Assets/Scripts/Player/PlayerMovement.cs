using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float movespeed;
    [SerializeField] private float crouchMovespeed = 10.0f;
    [SerializeField] private float walkMovespeed = 50.0f;
    [SerializeField] private float sprintMovespeed = 75.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movespeed = walkMovespeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

    }
}
