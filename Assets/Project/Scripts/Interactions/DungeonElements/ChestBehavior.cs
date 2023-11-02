using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour
{
    [Header("Chest lid")]
    public GameObject chestLid;

    [Header("Point to look inside chest")]
    public Transform inChestCameraTransform;

    [Header("Is chest currently open")]
    public bool chestOpen = false;

    GameObject mainCamera;
    PlayerMovement playerMovement;
    PauseMenu pauseMenu;

    private AudioSource openingChestSound;
    private AudioSource closingChestSound;

    private void Start() //get necessary components on awake
    {
        mainCamera = PlayerParams.Objects.playerCamera.gameObject;
        playerMovement = PlayerParams.Controllers.playerMovement;
        pauseMenu = PlayerParams.Controllers.pauseMenu;

        openingChestSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_OpenChest);
        openingChestSound.volume = 0.3f;
        closingChestSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CloseChest);
        closingChestSound.volume = 0.3f;
    }

    private void Update() //listen to chest close input if chest is open
    {
        if (chestOpen)
        {
            pauseMenu.ableToInteract = false;
            if (Input.GetKeyDown(KeyCode.Escape) && !PlayerParams.Variables.uiActive)
            {
                InteractChest();
            }
        }
    }

    public void InteractChest() //move camera to chest/player and start chest animation
    {
        StartCoroutine(MoveCamera());
        StartCoroutine(ChestAnimation());
    }

    IEnumerator ChestAnimation() //animating chest
    {
        if(chestOpen) //if chest is open then close lid
        {
            if (!closingChestSound.isPlaying) closingChestSound.Play();
            while (chestLid.transform.localRotation != Quaternion.Euler(0, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                chestLid.transform.localRotation = Quaternion.RotateTowards(chestLid.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }
            chestOpen = false;
            pauseMenu.ableToInteract = true;
        }
        else //else open lid
        {
            if(!openingChestSound.isPlaying) openingChestSound.Play();
            while (chestLid.transform.localRotation != Quaternion.Euler(-135, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                chestLid.transform.localRotation = Quaternion.RotateTowards(chestLid.transform.localRotation, Quaternion.Euler(-135, 0, 0), 5);
            }
            chestOpen = true;
            pauseMenu.ableToInteract = false;
        }
    }

    IEnumerator MoveCamera() //move camera to chest or to player
    {
        if(chestOpen) //if chest is open then move camera to player 
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
