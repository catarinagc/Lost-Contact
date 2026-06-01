using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Jammed Door Settings")]
    [SerializeField] private bool jammed = false;
    [SerializeField] private float jammedCoefficient = 5.0f;

    [Header("Door Positions")]
    [SerializeField] private Vector3 openDestination;
    [SerializeField] private Vector3 jammedOpenDestination;
    [SerializeField] private Vector3 closeDestination;

    [Header("Door Timing")]
    [SerializeField] private float timeToOpen = 3.0f;
    [SerializeField] private float timeStillOpen = 3.0f;
    [SerializeField] private float timeToClose = 1.0f;

    private float stillOpenTimer = 3.0f;
    private bool shouldLockAfterClosing = false;

    private Rigidbody rb;

    [Header("Door Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip jammedSound;
    [SerializeField] private float audioVolume = 0.8f;

    private enum DOOR_STATE
    {
        CLOSED,
        OPENING,
        OPENING_JAMMED,
        STOPPED_JAMMED,
        OPEN,
        CLOSING,
        LOCKED
    }

    [SerializeField] private DOOR_STATE doorState = DOOR_STATE.CLOSED;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (jammedCoefficient <= 0f)
            jammedCoefficient = 3.0f;

        stillOpenTimer = timeStillOpen;

        closeDestination = transform.position;
        openDestination = closeDestination + (transform.up * 6.0f);

        SetupJammedOpenDistance();
    }

    private void FixedUpdate()
    {
        switch (doorState)
        {
            case DOOR_STATE.OPENING:
                OpenDoor();
                break;

            case DOOR_STATE.OPENING_JAMMED:
                OpenJammedDoor();
                break;

            case DOOR_STATE.STOPPED_JAMMED:
                stillOpenTimer -= Time.fixedDeltaTime;

                if (stillOpenTimer <= 0f)
                {
                    StartClosing();
                }

                break;

            case DOOR_STATE.OPEN:
                stillOpenTimer -= Time.fixedDeltaTime;

                if (stillOpenTimer <= 0f)
                {
                    StartClosing();
                }

                break;

            case DOOR_STATE.CLOSING:
                CloseDoor();
                break;
        }
    }

    public bool IsLocked()
    {
        return doorState == DOOR_STATE.LOCKED;
    }

    public void SetupJammedOpenDistance()
    {
        float fullOpenDistance = Vector3.Distance(closeDestination, openDestination);
        float jammedStepDistance = fullOpenDistance / jammedCoefficient;

        jammedOpenDestination = closeDestination + (transform.up * jammedStepDistance);
    }

    public void SetJammed(bool isJammed)
    {
        jammed = isJammed;
    }

    public void InteractDoor()
    {
        if (doorState == DOOR_STATE.LOCKED)
        {
            return;
        }

        if (doorState == DOOR_STATE.CLOSED || doorState == DOOR_STATE.STOPPED_JAMMED)
        {
            if (!jammed)
            {
                PlayDoorSound(openSound);
                doorState = DOOR_STATE.OPENING;
            }
            else
            {
                if (jammedSound != null)
                    PlayDoorSound(jammedSound);
                else
                    PlayDoorSound(openSound);

                doorState = DOOR_STATE.OPENING_JAMMED;
            }
        }
    }

    public void OpenDoor()
    {
        float fullOpenDistance = Vector3.Distance(closeDestination, openDestination);
        float speed = fullOpenDistance / timeToOpen;

        Vector3 newPosition = Vector3.MoveTowards(
            rb.position,
            openDestination,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        if (Vector3.Distance(rb.position, openDestination) < 0.1f)
        {
            rb.MovePosition(openDestination);
            stillOpenTimer = timeStillOpen;
            doorState = DOOR_STATE.OPEN;
        }
    }

    public void OpenJammedDoor()
    {
        float fullOpenDistance = Vector3.Distance(closeDestination, openDestination);
        float speed = fullOpenDistance / timeToOpen;

        Vector3 newPosition = Vector3.MoveTowards(
            rb.position,
            jammedOpenDestination,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        // Reached the current jammed opening position
        if (Vector3.Distance(rb.position, jammedOpenDestination) < 0.1f)
        {
            rb.MovePosition(jammedOpenDestination);

            // If the jammed destination has reached the full open destination, the door is fully open
            if (Vector3.Distance(jammedOpenDestination, openDestination) < 0.1f)
            {
                rb.MovePosition(openDestination);
                stillOpenTimer = timeStillOpen;
                doorState = DOOR_STATE.OPEN;
            }
            else
            {
                // Otherwise, the door stops partially open
                float jammedStepDistance = fullOpenDistance / jammedCoefficient;

                jammedOpenDestination += transform.up * jammedStepDistance;

                if (Vector3.Distance(closeDestination, jammedOpenDestination) > fullOpenDistance)
                {
                    jammedOpenDestination = openDestination;
                }

                stillOpenTimer = timeStillOpen;
                doorState = DOOR_STATE.STOPPED_JAMMED;
            }
        }
    }

    private void StartClosing()
    {
        if (doorState == DOOR_STATE.CLOSING)
            return;

        PlayDoorSound(closeSound);
        doorState = DOOR_STATE.CLOSING;
    }

    public void CloseDoor()
    {
        float fullOpenDistance = Vector3.Distance(closeDestination, openDestination);
        float speed = fullOpenDistance / timeToClose;

        Vector3 newPosition = Vector3.MoveTowards(
            rb.position,
            closeDestination,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        if (Vector3.Distance(rb.position, closeDestination) < 0.1f)
        {
            rb.MovePosition(closeDestination);

            stillOpenTimer = timeStillOpen;
            SetupJammedOpenDistance();

            if (shouldLockAfterClosing)
            {
                doorState = DOOR_STATE.LOCKED;
                shouldLockAfterClosing = false;
                Debug.Log(name + " IS NOW LOCKED");
            }
            else
            {
                doorState = DOOR_STATE.CLOSED;
            }

            Debug.Log(name + " IS CLOSED");
        }
    }

    public void Unlock()
    {
        shouldLockAfterClosing = false; // ← add this
        doorState = DOOR_STATE.CLOSED;
    }

    public void Lock()
    {
        shouldLockAfterClosing = true;

        if (doorState == DOOR_STATE.CLOSED)
        {
            doorState = DOOR_STATE.LOCKED;
        }
        else
        {
            StartClosing();
        }
    }

    private void PlayDoorSound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, audioVolume);
    }
}
