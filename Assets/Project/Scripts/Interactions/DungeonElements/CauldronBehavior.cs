using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CauldronBehavior : InteractableBehavior
{
    [Header("Point to look inside cauldron")]
    [SerializeField] Transform _inCauldronCameraTransform;
    [SerializeField] List<Collider> _cauldronInteractionColliders;
    
    bool _isCauldronZoomed = false;
    GameObject mainCamera;
    PlayerMovement playerMovement;
    PauseMenu pauseMenu;

    private void Start() //get necessary components on start
    {
        mainCamera = PlayerParams.Objects.playerCamera.gameObject;
        playerMovement = PlayerParams.Controllers.playerMovement;
        pauseMenu = PlayerParams.Controllers.pauseMenu;
    }

    private void Update() //listen to chest close input if chest is open
    {
        if (_isCauldronZoomed)
        {
            pauseMenu.ableToInteract = false;
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.S)) && !PlayerParams.Variables.uiActive)
            {
                OnClick();
            }
        }
    }

    public void OnClick() //move camera to chest/player and start chest animation
    {
        if (!playerMovement.isMoving && !playerMovement.isRotating && !playerMovement.isLeaning)
        {
            StartCoroutine(MoveCamera());
        }
    }

    IEnumerator MoveCamera() //move camera to couldron or to player
    {
        if (_isCauldronZoomed) //if cauldron is zoomed in then move camera to player 
        {
            _isCauldronZoomed = false;
            pauseMenu.ableToInteract = false;

            while (mainCamera.transform.localPosition != PlayerParams.Variables.cameraStartingPosition || mainCamera.transform.localRotation != Quaternion.Euler(0, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.localPosition = Vector3.MoveTowards(mainCamera.transform.localPosition, PlayerParams.Variables.cameraStartingPosition, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }

            foreach (Collider col in _cauldronInteractionColliders) { col.enabled = true; }
            playerMovement.stopMovement = false;
        }
        else //else move camera to cauldron and stop player movement
        {
            playerMovement.stopMovement = true;
            foreach (Collider col in _cauldronInteractionColliders) { col.enabled = false; }

            Vector3 pivot = _inCauldronCameraTransform.parent.position; // lokalne (0,0,0)
            Vector3 from = Vector3.ProjectOnPlane(_inCauldronCameraTransform.position - pivot, Vector3.up);
            Vector3 to = Vector3.ProjectOnPlane(mainCamera.transform.position - pivot, Vector3.up);
            float angle = Mathf.Round(Vector3.SignedAngle(from, to, Vector3.up) / 90f) * 90f;
            _inCauldronCameraTransform.localPosition = Quaternion.Euler(0, angle, 0) * _inCauldronCameraTransform.localPosition;

            while (mainCamera.transform.position != _inCauldronCameraTransform.position || mainCamera.transform.localRotation != _inCauldronCameraTransform.localRotation)
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, _inCauldronCameraTransform.position, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, _inCauldronCameraTransform.localRotation, 4);
            }

            _isCauldronZoomed = true;
        }
    }
}
