using UnityEngine;

public class Door : MonoBehaviour
{
    //Variables for Jammed Doors
    [SerializeField] private bool jammed = false;
    [SerializeField] private float jammedCoefficient;
    //Open and Close Door Variables
    [SerializeField] private Vector3 openDestination;
    [SerializeField] private Vector3 jammedOpenDestination;
    [SerializeField] private Vector3 closeDestination;
    //Timers to Open and Close Doors
    [SerializeField] private float timeToOpen = 3.0f; //door opens in X seconds
    [SerializeField] private float timeStillOpen = 3.0f; //door stays open for X seconds
    private float stillOpenTimer = 3.0f; //when == 0, door begins closing
    [SerializeField] private float timeToClose = 1.0f; //door closes in X seconds
    private bool shouldLockAfterClosing = false;
    //
    private Rigidbody rb;
    //Tracking State of Door
    private enum DOOR_STATE {CLOSED, OPENING, OPENING_JAMMED, STOPPED_JAMMED, OPEN, CLOSING, LOCKED};
    [SerializeField] private DOOR_STATE doorState = DOOR_STATE.CLOSED;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jammedCoefficient = 3.0f;
        stillOpenTimer = timeStillOpen;
        //
        openDestination = transform.position + (transform.up * 6.0f);
        SetupJammedOpenDistance();
        closeDestination = transform.position;
        //
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    doorState = DOOR_STATE.CLOSING;
                }
                break;
            case DOOR_STATE.OPEN:
                stillOpenTimer -= Time.fixedDeltaTime;
                if (stillOpenTimer <= 0.0f)
                {
                    doorState = DOOR_STATE.CLOSING;
                }
                break;
            case DOOR_STATE.CLOSING:
                CloseDoor();
                break;
        }
    }

    public void SetupJammedOpenDistance()
    {
        jammedOpenDestination = transform.position + (transform.up / jammedCoefficient);
    }

    public void SetJammed(bool isJammed)
    {
        jammed = isJammed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.SetCanInteract(false);
            Debug.Log("PLAYER IS NOW IN RANGE OF " + name);

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (doorState == DOOR_STATE.LOCKED)
            {
                Debug.Log("LOCKED");
                return;
            }

            if (player.GetCanInteract() && 
                (doorState == DOOR_STATE.CLOSED || doorState == DOOR_STATE.STOPPED_JAMMED))
            {
                if (!jammed)
                {
                    doorState = DOOR_STATE.OPENING;
                }
                else 
                {
                    doorState = DOOR_STATE.OPENING_JAMMED;   
                }
                player.SetCanInteract(false);
            }           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.SetCanInteract(false);
            Debug.Log("PLAYER IS NO LONGER ON RANGE OF " + name);
        }
    }

    public void OpenDoor()
    {
        Debug.Log("OPENING NON-JAMMED " + name);
        float speed = Vector3.Distance(closeDestination, openDestination) / timeToOpen;
        Vector3 newPosition = Vector3.MoveTowards(rb.position, openDestination, speed * Time.fixedDeltaTime);
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
        Debug.Log("OPENING JAMMED " + name);
        float speed = Vector3.Distance(closeDestination, jammedOpenDestination);
        Vector3 newPosition = Vector3.MoveTowards(rb.position, jammedOpenDestination, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        //Fully Open
        if (Vector3.Distance(closeDestination, openDestination) < 0.05f)
        {
            rb.MovePosition(openDestination);
            doorState = DOOR_STATE.OPEN;
            stillOpenTimer = timeStillOpen;
        }
        //Still Jammed
        else
        {
            jammedOpenDestination += transform.up;
            if (Vector3.Distance(closeDestination, jammedOpenDestination) >
                Vector3.Distance(closeDestination, openDestination))
            {
                jammedOpenDestination = openDestination;
            }
            doorState = DOOR_STATE.STOPPED_JAMMED;
            stillOpenTimer = timeStillOpen; //begin close door timer even if jammed
        }
    }

    public void CloseDoor()
    {
        Debug.Log("CLOSING " + name);

        float speed = Vector3.Distance(openDestination, closeDestination) / timeToClose;
        Vector3 newPosition = Vector3.MoveTowards(rb.position, closeDestination, speed * Time.fixedDeltaTime);
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
            //doorState = DOOR_STATE.CLOSED;
            Debug.Log(name + " IS CLOSED ");
        }
    }

    public void Unlock()
    {
        doorState = DOOR_STATE.CLOSED;
    }

    public void Lock()
    {
        shouldLockAfterClosing = true;

        // If already closed, lock immediately
        if (doorState == DOOR_STATE.CLOSED)
        {
            doorState = DOOR_STATE.LOCKED;
        }
        else
        {
            // Otherwise begin closing
            doorState = DOOR_STATE.CLOSING;
        }
    }
}
