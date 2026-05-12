using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    //
    [SerializeField] private float movespeed;
    [SerializeField] private float crouchMovespeed = 1.0f;
    [SerializeField] private float walkSlowMovespeed = 2.0f;
    [SerializeField] private float walkMovespeed = 5.0f;
    [SerializeField] private float sprintMovespeed = 7.0f;
    [SerializeField] private float mouseSensitivity = 100.0f;
    private Vector3 moveDirection;
    private float horizontal;
    private float vertical;
    //
    private float mouseX;
    private float mouseY;
    private float xRotation;
    //
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Vector3 crouchedScale;
    private Vector3 nonCrouchedScale;
    //
    private bool canInteract = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        //
        movespeed = walkMovespeed;
        crouchedScale = new Vector3(1.0f, 0.5f, 1.0f);
        nonCrouchedScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        MovementInput();
        InteractionInput();
        MouseLookAround();
    }

    private void FixedUpdate()
    {
        
        MovePlayer();
    }

    private void MovementInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movespeed = sprintMovespeed;
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            movespeed = walkSlowMovespeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            movespeed = crouchMovespeed;
            capsuleCollider.transform.localScale = crouchedScale;
            transform.localScale = crouchedScale;
        }
        else
        {
            movespeed = walkMovespeed;
            capsuleCollider.transform.localScale = nonCrouchedScale;
            transform.localScale = nonCrouchedScale;
        }
    }

    private void InteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            canInteract = true;
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
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //
        transform.Rotate(Vector3.up * mouseX);
    }

    public bool GetCanInteract()
    {
        return canInteract;
    }

    public void SetCanInteract(bool newValue)
    {
        canInteract = newValue;
    }
}
