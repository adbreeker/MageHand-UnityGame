using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : InteractableBehavior
{
    [Header("Chest fragments:")]
    public GameObject chestLid;
    public GameObject chestCore;

    [Header("Point to look inside chest")]
    public Transform inChestCameraTransform;

    bool isChestOpen = false;
    GameObject mainCamera;
    PlayerMovement playerMovement;
    PauseMenu pauseMenu;

    EventInstance openingChestSound;
    EventInstance closingChestSound;

    protected override void Start() //get necessary components on start
    {
        base.Start();
        mainCamera = PlayerParams.Objects.playerCamera.gameObject;
        playerMovement = PlayerParams.Controllers.playerMovement;
        pauseMenu = PlayerParams.Controllers.pauseMenu;
    }

    private void Update() //listen to chest close input if chest is open
    {
        if (isChestOpen)
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
        if(!playerMovement.isMoving && !playerMovement.isRotating && !playerMovement.isLeaning)
        {
            StartCoroutine(MoveCamera());
            StartCoroutine(ChestAnimation());
        }
    }

    IEnumerator ChestAnimation() //animating chest
    {
        if(isChestOpen) //if chest is open then close lid
        {
            if (!closingChestSound.isValid())
            {
                closingChestSound = GameParams.Managers.audioManager.PlayOneShotSpatialized(GameParams.Managers.fmodEvents.SFX_CloseChest, transform);
            }
            while (chestLid.transform.localRotation != Quaternion.Euler(0, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                chestLid.transform.localRotation = Quaternion.RotateTowards(chestLid.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }
            isChestOpen = false;
            pauseMenu.ableToInteract = true;
        }
        else //else open lid
        {
            if (!openingChestSound.isValid())
            {
                openingChestSound = GameParams.Managers.audioManager.PlayOneShotSpatialized(GameParams.Managers.fmodEvents.SFX_OpenChest, transform);
            }
            while (chestLid.transform.localRotation != Quaternion.Euler(-135, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                chestLid.transform.localRotation = Quaternion.RotateTowards(chestLid.transform.localRotation, Quaternion.Euler(-135, 0, 0), 5);
            }
            isChestOpen = true;
            pauseMenu.ableToInteract = false;
        }
    }

    IEnumerator MoveCamera() //move camera to chest or to player
    {
        if(isChestOpen) //if chest is open then move camera to player 
        {
            while (mainCamera.transform.localPosition != PlayerParams.Variables.cameraStartingPosition || mainCamera.transform.localRotation != Quaternion.Euler(0,0,0))
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.localPosition = Vector3.MoveTowards(mainCamera.transform.localPosition, PlayerParams.Variables.cameraStartingPosition, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }
            playerMovement.stopMovement = false;
        }
        else //else move camera to chest and stop player movement
        {
            playerMovement.stopMovement = true;
            while (mainCamera.transform.position != inChestCameraTransform.position || mainCamera.transform.localRotation != inChestCameraTransform.localRotation)
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, inChestCameraTransform.position, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, inChestCameraTransform.localRotation, 4);
            }
        }
    }
}
