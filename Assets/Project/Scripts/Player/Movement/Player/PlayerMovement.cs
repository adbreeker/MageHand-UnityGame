using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Current tile:")]
    /// <summary> Transform of tile on which player is currently staying  </summary>
    public Transform currentTile;
    /// <summary> Position of player on current tile  </summary>
    public Vector3 currentOnTilePos;

    [Header("Movement")]
    public float movementSpeed = 7f;
    public float tilesLenght = 4f;
    public LayerMask obstacleMask;
    [HideInInspector] public bool isMoving = false;
    Vector3 _destination;
    float _distancePassed = 0;

    [Header("Rotation")]
    public float rotationSpeed = 300f;
    [HideInInspector] public bool isRotating = false;
    float _remainingRotation;

    [Header("Lean over")]
    public float leanSpeed = 200f;
    public float leanAngle = 60f;
    [HideInInspector] public bool isLeaning = false;

    [Header("Movement-interfering options")]
    public bool stopMovement = false;
    public bool ghostmodeActive = false;

    [Header("Special effects")]
    [SerializeField] GameObject _teleportationEffect;

    //enqueuing input
    private Vector3 _movementInputQueue = Vector3.zero;
    private float _rotationInputQueue = 0;

    //sound
    private string _dungeonCubeTag = "DungeonCube";
    private string _tunnelCubeTag = "TunnelCube";

    private EventInstance _leanSound;
    private EventInstance _getUpSound;
    private AudioManager AudioManager => GameParams.Managers.audioManager;

    private void Awake()
    {
        currentTile = RaycastCurrentTile();
        currentOnTilePos = transform.position;
    }

    void Update() //listen to movement inputs
    {
        MovementQueue();
        RotationQueue();
        MovementKeysListener(0, 0);
        RotationKeyListener(0);
        LeanKeyListener();
    }

    Transform RaycastCurrentTile()
    {
        RaycastHit[] potentialTile = Physics.RaycastAll(transform.position, Vector3.down, 4.0f);
        foreach (RaycastHit hit in potentialTile)
        {
            if(hit.transform.tag == _dungeonCubeTag || hit.transform.tag == _tunnelCubeTag)
            {
                return hit.transform;
            }
        }

        return null;
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
            _destination = transform.position + direction * tilesLenght;
            _destination.y = transform.position.y;

            if (CanMove())
            {
                _distancePassed = 0;
                isMoving = true;
            }
        }

        //move the player towards the destination
        if (isMoving)
        {
            Vector3 prePos = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, _destination, movementSpeed * Time.unscaledDeltaTime * (PlayerParams.Controllers.pauseMenu.freezeTime ? 0 : 1));
            _distancePassed += Vector3.Distance(prePos, transform.position);
            if(_distancePassed > 0.5f * tilesLenght) 
            { 
                WalkSound(); 
                _distancePassed = 0; 
            }

            if (transform.position == _destination)
            {
                isMoving = false; //stop moving when the destination is reached
                currentTile = RaycastCurrentTile();
                currentOnTilePos = transform.position;
            }
            if (!CanMove())
            {
                _destination = currentOnTilePos; //back to previous tile if collide with something
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
        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x, 1.25f, transform.position.z), 0.8f, obstacleMask, QueryTriggerInteraction.Ignore);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Wall" || collider.gameObject.tag == "Obstacle")
            {
                //if obstacle near player then can't move
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_PlayerCollision);
                //Destroy(collisionSound.gameObject, collisionSound.clip.length);

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

    void WalkSound()
    {
        if (RaycastCurrentTile().tag == _dungeonCubeTag)
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_PlayerStepsDungeon);
        else if (RaycastCurrentTile().tag == _tunnelCubeTag)
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_PlayerStepsTunnel);
    }

    void RotationKeyListener(float rotationQueue)
    {
        //check if no rotation is enqueued
        if(rotationQueue != 0 && !isRotating)
        {
            isRotating = true;

            if (rotationQueue == 90) RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_PlayerRotateRight);
            else if (rotationQueue == -90) RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_PlayerRotateLeft);

            _remainingRotation = rotationQueue;
        }

        //get target rotation
        if (Input.GetKeyDown(KeyCode.E) && !isRotating && !MovementInterfering())
        {
            isRotating = true;
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_PlayerRotateRight);
            _remainingRotation = 90;
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating && !MovementInterfering())
        {
            isRotating = true;
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_PlayerRotateLeft);
            _remainingRotation = -90;
        }

        //rotate towards target rotation
        if (isRotating)
        {
            float rotationStep = rotationSpeed * Time.unscaledDeltaTime * (PlayerParams.Controllers.pauseMenu.freezeTime ? 0 : 1);
            if(Mathf.Abs(_remainingRotation) < rotationStep) { rotationStep = _remainingRotation; }
            else { rotationStep *= Mathf.Sign(_remainingRotation); }

            _remainingRotation -= rotationStep;
            transform.Rotate(0f, rotationStep, 0f);

            if (_remainingRotation == 0)
            {
                isRotating = false;
            }
        }
    }

    void LeanKeyListener()
    {
        if(Input.GetKey(KeyCode.LeftControl) && !MovementInterfering())
        {

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (AudioManager.IsPlaying(_getUpSound)) _getUpSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _leanSound = AudioManager.PlayOneShotReturnInstance(GameParams.Managers.fmodEvents.SFX_PlayerLean);
            }

            isLeaning = true;

            Transform cam = PlayerParams.Objects.playerCamera.transform;
            cam.localRotation = Quaternion.RotateTowards(cam.localRotation, Quaternion.Euler(leanAngle, 0, 0), leanSpeed * Time.unscaledDeltaTime);
        }
        else if(isLeaning)
        {
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                if (AudioManager.IsPlaying(_leanSound)) _leanSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _getUpSound = AudioManager.PlayOneShotReturnInstance(GameParams.Managers.fmodEvents.SFX_PlayerGetUp);
            }

            Transform cam = PlayerParams.Objects.playerCamera.transform;

            cam.localRotation = Quaternion.RotateTowards(cam.localRotation, Quaternion.Euler(0, 0, 0), leanSpeed * Time.unscaledDeltaTime);
            if(cam.localRotation.eulerAngles == Vector3.zero)
            {
                isLeaning = false;
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
                _movementInputQueue.x = Input.GetAxisRaw("Horizontal");
                _movementInputQueue.z = Input.GetAxisRaw("Vertical");
            }
        }
        else //initializing queued movement after last move
        {
            if(_movementInputQueue != Vector3.zero)
            {
                MovementKeysListener(_movementInputQueue.x, _movementInputQueue.z);
                _movementInputQueue = Vector3.zero;
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
                    _rotationInputQueue = 90;
                }
                if (Input.GetKeyDown(KeyCode.Q)) //rotate queue left
                {
                    _rotationInputQueue = -90;
                }
            }
        }
        else //initializing enqueued rotation after last rotation
        {
            if (_rotationInputQueue != 0)
            {
                RotationKeyListener(_rotationInputQueue);
                _rotationInputQueue = 0;
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

    //teleportation methods

    public void TeleportTo(Vector3 tpDestination, float tpRotation) //teleport to destination and stop movement enqued before teleportation
    {
        _destination = tpDestination;
        transform.position = tpDestination;
        isMoving = false;

        _remainingRotation = 0f;
        transform.rotation = Quaternion.Euler(0f, tpRotation, 0f);
        isRotating = false;


        _movementInputQueue = Vector3.zero;
        _rotationInputQueue = 0;

        currentTile = RaycastCurrentTile();
        currentOnTilePos = transform.position;
    }
    public void TeleportTo(Vector3 tpDestination, float tpRotation, Color? tpEffectColor)
    {
        RuntimeManager.PlayOneShotAttached(GameParams.Managers.fmodEvents.SFX_Teleport, PlayerParams.Objects.player);

        _destination = tpDestination;
        transform.position = tpDestination;
        isMoving = false;

        _remainingRotation = 0f;
        transform.rotation = Quaternion.Euler(0f, tpRotation, 0f);
        isRotating = false;

        _movementInputQueue = Vector3.zero;
        _rotationInputQueue = 0;

        currentTile = RaycastCurrentTile();
        currentOnTilePos = transform.position;

        if (tpEffectColor != null) 
        {
            Instantiate(_teleportationEffect, transform)
                    .GetComponent<ParticlesColor>().ChangeColorOfEffect(tpEffectColor.Value);
        }
        else
        {
            Instantiate(_teleportationEffect, transform);
        }
    }
}
