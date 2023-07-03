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
    PlayerMovement pr;

    private void Awake() //get necessary components on awake
    {
        mainCamera = Camera.main.gameObject;
        pr = mainCamera.GetComponentInParent<PlayerMovement>();
    }

    private void Update() //listen to chest close input if chest is open
    {
        if(chestOpen)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
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
            while (chestLid.transform.localRotation != Quaternion.Euler(0, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                chestLid.transform.localRotation = Quaternion.RotateTowards(chestLid.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }
            chestOpen = false;
        }
        else //else open lid
        {
            while (chestLid.transform.localRotation != Quaternion.Euler(-135, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                chestLid.transform.localRotation = Quaternion.RotateTowards(chestLid.transform.localRotation, Quaternion.Euler(-135, 0, 0), 5);
            }
            chestOpen = true;
        }
    }

    Vector3 playerCameraPos = new Vector3(0,1,0);
    IEnumerator MoveCamera() //move camera to chest or to player
    {
        if(chestOpen) //if chest is open then move camera to player 
        {
            while (mainCamera.transform.localPosition != playerCameraPos || mainCamera.transform.localRotation != Quaternion.Euler(0,0,0))
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.localPosition = Vector3.MoveTowards(mainCamera.transform.localPosition, playerCameraPos, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }
            pr.stopMovement = false;
        }
        else //else move camera to chest and stop player movement
        {
            pr.stopMovement = true;
            while (mainCamera.transform.position != inChestCameraTransform.position || mainCamera.transform.localRotation != inChestCameraTransform.localRotation)
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, inChestCameraTransform.position, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, inChestCameraTransform.localRotation, 4);
            }

        }
    }
}
