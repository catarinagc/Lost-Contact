using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private AudioSource audioSource;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Settings")]
    [SerializeField] private float stepInterval = 0.45f;
    [SerializeField] private float minMoveSpeed = 0.05f;
    [SerializeField] private float volume = 0.8f;

    [Header("Debug")]
    [SerializeField] private bool requireGrounded = false;
    [SerializeField] private bool showDebugLogs = true;

    private float stepTimer;
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;

        if (showDebugLogs)
        {
            Debug.Log("FootstepSFX started on: " + gameObject.name);
            Debug.Log("CharacterController assigned: " + (characterController != null));
            Debug.Log("AudioSource assigned: " + (audioSource != null));
            Debug.Log("Footstep clips count: " + footstepClips.Length);
        }
    }

    private void Update()
    {
        if (characterController == null || audioSource == null)
            return;

        if (footstepClips == null || footstepClips.Length == 0)
            return;

        Vector3 currentPosition = transform.position;

        Vector3 horizontalMovement = currentPosition - lastPosition;
        horizontalMovement.y = 0f;

        float movementSpeed = horizontalMovement.magnitude / Time.deltaTime;

        bool isGrounded = characterController.isGrounded;
        bool isMoving = movementSpeed > minMoveSpeed;

        if (showDebugLogs && isMoving)
        {
            Debug.Log("Moving. Speed: " + movementSpeed + " | Grounded: " + isGrounded);
        }

        bool canPlayFootstep = isMoving;

        if (requireGrounded)
            canPlayFootstep = isMoving && isGrounded;

        if (canPlayFootstep)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        lastPosition = currentPosition;
    }

    private void PlayFootstep()
    {
        int randomIndex = Random.Range(0, footstepClips.Length);

        if (footstepClips[randomIndex] == null)
        {
            Debug.LogWarning("Footstep clip at index " + randomIndex + " is empty.");
            return;
        }

        Debug.Log("Playing footstep: " + footstepClips[randomIndex].name);

        audioSource.PlayOneShot(footstepClips[randomIndex], volume);
    }
}