using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    //
    [SerializeField] private float movespeed;
    [SerializeField] private float walkSlowMovespeed = 2.0f;
    [SerializeField] private float walkMovespeed = 5.0f;
    [SerializeField] private float sprintMovespeed = 7.0f;
    [SerializeField] private float mouseSensitivity = 100.0f;
    private Vector3 moveDirection;
    private float horizontal;
    private float vertical;
    //
    private float mouseY;
    private float mouseX;
    private float xRotation;
    //
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        movespeed = walkMovespeed;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectMovementInput();
        MouseLookAround();
    }

    private void FixedUpdate()
    {
        
        MovePlayer();
    }

    private void DetectMovementInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movespeed = sprintMovespeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            movespeed = walkSlowMovespeed;
        }
        else
        {
            movespeed = walkMovespeed;
        }
    }

    private void MovePlayer()
    {
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        moveDirection = (forward * vertical + right * horizontal).normalized;
        //
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * movespeed;
        velocity.z = moveDirection.z * movespeed;

        rb.linearVelocity = velocity;
    }

    private void MouseLookAround()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //
        transform.Rotate(Vector3.up * mouseX);
    }
}
