using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 5f;
    public float tilesLenght = 4f;
    public LayerMask obstacleMask;

    private Vector3 destination;
    private Vector3 previousTile;
    private bool isMoving = false;

    [Header("Rotation")]
    public float rotationSpeed = 360f;
    public float rotationThreshold = 0.01f;
    private bool isRotating = false;
    private Quaternion targetRotation;

    [Header("Movement-interfering options")]
    public bool uiActive = false;
    public bool stopMovement = false;
    public bool ghostmodeActive = false;
    

    //Enqueuing
    private Vector3 movementInputQueue = Vector3.zero;
    private float rotationInputQueue = 0;

    void Update()
    {
        MovementQueue();
        RotationQueue();
        MovementKeysListener(0, 0);
        RotationKeyListener(0);
    }

    void MovementKeysListener(float horizontalInputQueue, float verticalInputQueue)
    {
        float horizontalInput = horizontalInputQueue;
        float verticalInput = verticalInputQueue;

        // Check if no movement is enqueued
        if (horizontalInputQueue == 0 && verticalInputQueue ==0 && !uiActive && !stopMovement)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        if ((horizontalInput != 0 || verticalInput != 0) && !isMoving)
        {
            // Calculate the position of the tile the player is moving towards
            Vector3 direction = SingleDirectionNormalization(transform.right * horizontalInput + transform.forward * verticalInput);
            destination = transform.position + direction * tilesLenght;
            destination.y = transform.position.y;
            previousTile = transform.position;
            if (CanMove())
            {
                isMoving = true;
            }
        }

        // Move the player towards the destination
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            if (transform.position == destination)
            {
                isMoving = false; // Stop moving when the destination is reached
            }
            if (!CanMove())
            {
                destination = previousTile; // Back to previous tile if collide with something
            }
        }
    }

    // Check if the player can move to a given destination
    bool CanMove()
    {
        if(ghostmodeActive)
        {
            return true;
        }

        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x, 1.25f, transform.position.z), 0.8f, obstacleMask);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Wall" || collider.gameObject.tag == "Obstacle")
            {
                return false;
            }
        }
        return true;
    }

    //Normalizing and single directioning (no diagonal movement)
    Vector3 SingleDirectionNormalization(Vector3 direction)
    {
        direction = direction.normalized;
        int xsign, zsign;

        if (direction.x >= 0) { xsign = 1; }
        else { xsign = -1; }

        if (direction.z >= 0) { zsign = 1; }
        else { zsign = -1; }

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.z))
        {
            direction.x = 1 * xsign;
            direction.z = 0;
        }
        else
        {
            direction.x = 0;
            direction.z = 1 * zsign;
        }
        return direction;
    }

    void RotationKeyListener(float rotationQueue)
    {
        //Check if no rotation is enqueued
        if(rotationQueue != 0 && !isRotating)
        {
            isRotating = true;
            targetRotation = transform.rotation * Quaternion.Euler(0, rotationQueue, 0);
        }

        // Get target rotation
        if (Input.GetKeyDown(KeyCode.E) && !isRotating && !uiActive && !stopMovement)
        {
            isRotating = true;
            targetRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating && !uiActive && !stopMovement)
        {
            isRotating = true;
            targetRotation = transform.rotation * Quaternion.Euler(0, -90, 0);
        }

        // Rotate towards target rotation
        if (isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation) < rotationThreshold)
            {
                isRotating = false;
                transform.rotation = targetRotation;
            }
        }
    }

    void MovementQueue()
    {
        //Enqueuing movement if input while moving
        if(isMoving && !uiActive && !stopMovement)
        {
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                movementInputQueue.x = Input.GetAxisRaw("Horizontal");
                movementInputQueue.z = Input.GetAxisRaw("Vertical");
            }
        }
        //Initializing queued movement after last move
        else
        {
            if(movementInputQueue != Vector3.zero)
            {
                MovementKeysListener(movementInputQueue.x, movementInputQueue.z);
                movementInputQueue = Vector3.zero;
            }
        }
    }

    void RotationQueue()
    {
        //Enqueuing rotation if input while rotating
        if (isRotating && !uiActive && !stopMovement)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    rotationInputQueue = 90;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    rotationInputQueue = -90;
                }
            }
        }
        //Initializing queued rotation after last rotation
        else
        {
            if (rotationInputQueue != 0)
            {
                RotationKeyListener(rotationInputQueue);
                rotationInputQueue = 0;
            }
        }
    }

    public void TeleportTo(Vector3 tpDestination)
    {
        destination = tpDestination;
        transform.position = tpDestination;
        isMoving = false;
        movementInputQueue = Vector3.zero;
    }

}
