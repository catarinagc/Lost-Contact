using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
public class Player : MonoBehaviour
{
    //[SerializeField] private Camera playerCamera;
    //
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    private float moveSpeed = 5.0f;
    [SerializeField] private float crouchMovespeed = 1.0f;
    [SerializeField] private float walkMovespeed = 5.0f;
    [SerializeField] private float sprintMovespeed = 10.0f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    public float gravity = -9.81f;
    [SerializeField] List<GameObject> possibleStartPos;
    [SerializeField] private GameObject doorInfoText;
    private Coroutine currentDoorTextRoutine;
    private Vector3 moveDirection;
    private float horizontal;
    private float vertical;
    //
    private float mouseX;
    private float mouseY;
    private float xRotation;
    private float pitch;
    public Transform camTransform;
    public Camera camera;
    //
    // private Rigidbody rb;
    // private CapsuleCollider capsuleCollider;
    private Vector3 crouchedScale;
    private Vector3 nonCrouchedScale;
    //
    private bool canInteract = false;
    private bool canMove = true;
    private CharacterController controller;
    public InputActionReference sprintAction;
    public InputActionReference crouchAction;

    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float normalHeight = 2f;

    private bool isCrouching = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        moveAction.action.Enable();
        lookAction.action.Enable();
        moveSpeed = walkMovespeed;
        crouchedScale = new Vector3(1.0f, 0.5f, 1.0f);
        nonCrouchedScale = new Vector3(1.0f, 1.0f, 1.0f);
        ChooseStartPos();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleLook();
        InteractionInput();
    }

    void HandleMovement()
    {
        if (!canMove)
            return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        bool isSprinting = sprintAction.action.IsPressed();

        isCrouching = crouchAction.action.IsPressed();

        if (isCrouching)
        {
            moveSpeed = crouchMovespeed;
        }
        else if (isSprinting)
        {
            moveSpeed = sprintMovespeed;
        }
        else
        {
            moveSpeed = walkMovespeed;
        }

        controller.height = isCrouching ? crouchHeight : normalHeight;

        // lower camera while crouching
        Vector3 camPos = camTransform.localPosition;
        camPos.y = isCrouching ? 0.25f : 1f;
        camTransform.localPosition = camPos;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 moveDirection = (forward * input.y) + (right * input.x);

        Vector3 finalMove = moveDirection.normalized * moveSpeed;

        // Gravity
        if (controller.isGrounded && finalMove.y < 0)
        {
            finalMove.y = -2f;
        }

        finalMove.y += gravity * Time.deltaTime;

        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (!canMove)
            return;
        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

        // Horizontal (Yaw)
        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);

        // Vertical (Pitch)
        pitch -= mouseDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Apply pitch to the camera only
        camTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void ChooseStartPos()
    {
        int randomIndex = Random.Range(0, possibleStartPos.Count);
        GameObject chosenPos = possibleStartPos[randomIndex];
        transform.position = chosenPos.transform.position;
        transform.rotation = chosenPos.transform.rotation;
    }

    private IEnumerator ShowDoorText()
    {
        doorInfoText.SetActive(true);

        yield return new WaitForSeconds(2f);

        doorInfoText.SetActive(false);
    }

    // private void MovementInput()
    // {
    //     horizontal = Input.GetAxisRaw("Horizontal");
    //     vertical = Input.GetAxisRaw("Vertical");

    //     if (Input.GetKey(KeyCode.LeftShift))
    //     {
    //         movespeed = sprintMovespeed;
    //     }
    //     else if (Input.GetKey(KeyCode.LeftControl))
    //     {
    //         movespeed = crouchMovespeed;
    //         capsuleCollider.transform.localScale = crouchedScale;
    //         transform.localScale = crouchedScale;
    //     }
    //     else
    //     {
    //         movespeed = walkMovespeed;
    //         capsuleCollider.transform.localScale = nonCrouchedScale;
    //         transform.localScale = nonCrouchedScale;
    //     }
    // }

    private void InteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            //canInteract = true;
            //pensar depois se vale a pena fazer tudo com triggers ou com rays
            //por enquanto manter objs separados para nao fazer overlap das interacoes
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray =
            camera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            Door door = hit.collider.GetComponentInParent<Door>();

            if (door != null)
            {
                Debug.Log("here");
                if (door.IsLocked())
                {
                    // Prevent coroutine spam
                    if (currentDoorTextRoutine != null)
                    {
                        StopCoroutine(currentDoorTextRoutine);
                    }

                    currentDoorTextRoutine =
                        StartCoroutine(ShowDoorText());

                    return;
                }
                else
                {
                    door.InteractDoor();
                }
            }

            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                canMove = false;
                interactable.Interact();
            }
        }
    }

    // private void MovePlayer()
    // {
    //     if (!canMove)
    //     {
    //         rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    //         return;
    //     }
    //     Vector3 forward = playerCamera.transform.forward;
    //     Vector3 right = playerCamera.transform.right;
    //     forward.y = 0f;
    //     right.y = 0f;
    //     forward.Normalize();
    //     right.Normalize();
    //     moveDirection = (forward * vertical + right * horizontal).normalized;
    //     //
    //     Vector3 velocity = rb.linearVelocity;
    //     velocity.x = moveDirection.x * movespeed;
    //     velocity.z = moveDirection.z * movespeed;

    //     rb.linearVelocity = velocity;
    // }

    // private void MouseLookAround()
    // {
    //     if (!canMove)
    //         return;
    //     mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
    //     mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
    //     //
    //     xRotation -= mouseY;
    //     xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    //     camTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    //     //
    //     transform.Rotate(Vector3.up * mouseX);
    // }

    public bool GetCanInteract()
    {
        return canInteract;
    }

    public void SetCanInteract(bool newValue)
    {
        canInteract = newValue;
    }

    public void SetMovementEnabled()
    {
        canMove = true;
    }

    public bool getIsCrouching()
    {
        return isCrouching || moveAction.action.ReadValue<Vector2>() == Vector2.zero;
    }
}
