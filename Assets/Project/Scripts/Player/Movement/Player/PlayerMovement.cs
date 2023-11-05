using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 7f;
    public float tilesLenght = 4f;
    public LayerMask obstacleMask;
    
    [HideInInspector] public Vector3 destination;
    [HideInInspector] public Vector3 previousTile;
    [HideInInspector] public bool isMoving = false;

    [Header("Rotation")]
    public float rotationSpeed = 300f;
    public float rotationThreshold = 0.01f;
    private bool isRotating = false;
    private Quaternion targetRotation;

    [Header("Movement-interfering options")]
    public bool stopMovement = false;
    public bool ghostmodeActive = false;
    

    //enqueuing input
    private Vector3 movementInputQueue = Vector3.zero;
    private float rotationInputQueue = 0;

    //steps sounds
    private bool lastWasStep1 = false;
    private AudioSource stepStone1;
    private AudioSource stepStone2;

    void Start()
    {       
        stepStone1 = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_StepStone1);
        stepStone2 = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_StepStone2);
    }

    void Update() //listen to movement inputs
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

        //check if no movement is enqueued
        if (horizontalInputQueue == 0 && verticalInputQueue ==0 && !MovementInterfering())
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        if ((horizontalInput != 0 || verticalInput != 0) && !isMoving)
        {
            //calculate the position of the tile the player is moving towards
            Vector3 direction = SingleDirectionNormalization(transform.right * horizontalInput + transform.forward * verticalInput);
            destination = transform.position + direction * tilesLenght;
            destination.y = transform.position.y;
            previousTile = transform.position;
            if (CanMove())
            {
                isMoving = true;

                if (lastWasStep1)
                {
                    stepStone2.PlayDelayed(0.2f);
                    lastWasStep1 = false;
                }
                else
                {
                    stepStone1.PlayDelayed(0.2f);
                    lastWasStep1 = true;
                }
            }
        }

        //move the player towards the destination
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            if (transform.position == destination)
            {
                isMoving = false; //stop moving when the destination is reached
            }
            if (!CanMove())
            {
                destination = previousTile; //back to previous tile if collide with something
            }
        }
    }

    //check if the player can move to a given destination
    bool CanMove()
    {
        if(ghostmodeActive) //if ghostmode is active then allways can move
        {
            return true;
        }

        //get obstacles near player
        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x, 1.25f, transform.position.z), 0.8f, obstacleMask);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Wall" || collider.gameObject.tag == "Obstacle")
            {
                //if obstacle near player then can't move
                return false;
            }
        }
        return true;
    }

    //normalizing and single directioning (no diagonal movement)
    Vector3 SingleDirectionNormalization(Vector3 direction)
    {
        direction = direction.normalized; //direction normalization
        int xsign, zsign;

        //geting sign of x and z variable
        if (direction.x >= 0) { xsign = 1; }
        else { xsign = -1; }

        if (direction.z >= 0) { zsign = 1; }
        else { zsign = -1; }

        //single directioning (dominant is direction we are closer to)
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
        //check if no rotation is enqueued
        if(rotationQueue != 0 && !isRotating)
        {
            isRotating = true;
            targetRotation = transform.rotation * Quaternion.Euler(0, rotationQueue, 0);
        }

        //get target rotation
        if (Input.GetKeyDown(KeyCode.E) && !isRotating && !MovementInterfering())
        {
            isRotating = true;
            targetRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating && !MovementInterfering())
        {
            isRotating = true;
            targetRotation = transform.rotation * Quaternion.Euler(0, -90, 0);
        }

        //rotate towards target rotation
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
        //enqueuing movement if input while moving
        if(isMoving && !MovementInterfering())
        {
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                movementInputQueue.x = Input.GetAxisRaw("Horizontal");
                movementInputQueue.z = Input.GetAxisRaw("Vertical");
            }
        }
        else //initializing queued movement after last move
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
        //enqueuing rotation if input while rotating
        if (isRotating && !MovementInterfering())
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
            {
                if (Input.GetKeyDown(KeyCode.E)) //rotate queue right
                {
                    rotationInputQueue = 90;
                }
                if (Input.GetKeyDown(KeyCode.Q)) //rotate queue left
                {
                    rotationInputQueue = -90;
                }
            }
        }
        else //initializing enqueued rotation after last rotation
        {
            if (rotationInputQueue != 0)
            {
                RotationKeyListener(rotationInputQueue);
                rotationInputQueue = 0;
            }
        }
    }

    bool MovementInterfering()
    {
        if(stopMovement || PlayerParams.Variables.uiActive)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TeleportTo(Vector3 tpDestination) //teleport to destination and stop movement enqued before teleportation
    {
        destination = tpDestination;
        transform.position = tpDestination;
        isMoving = false;
        movementInputQueue = Vector3.zero;
    }

}
