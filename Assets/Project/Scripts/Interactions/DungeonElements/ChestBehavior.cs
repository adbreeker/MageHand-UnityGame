using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour
{
    public GameObject chestLid;
    public Transform inChestCameraTransform;

    GameObject mainCamera;
    PlayerMovement pr;

    public bool chestOpen = false;

    private void Awake()
    {
        mainCamera = Camera.main.gameObject;
        pr = mainCamera.GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        if(chestOpen)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                InteractChest();
            }
        }
    }

    public void InteractChest()
    {
        StartCoroutine(MoveCamera());
        StartCoroutine(ChestAnimation());
    }

    IEnumerator ChestAnimation()
    {
        if(chestOpen)
        {
            while (chestLid.transform.localRotation != Quaternion.Euler(0, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                chestLid.transform.localRotation = Quaternion.RotateTowards(chestLid.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }
            chestOpen = false;
        }
        else
        {
            while (chestLid.transform.localRotation != Quaternion.Euler(-135, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                chestLid.transform.localRotation = Quaternion.RotateTowards(chestLid.transform.localRotation, Quaternion.Euler(-135, 0, 0), 5);
            }
            chestOpen = true;
        }
    }

    Vector3 previousCameraPos;
    IEnumerator MoveCamera()
    {
        if(chestOpen)
        {
            while (mainCamera.transform.localPosition != previousCameraPos || mainCamera.transform.localRotation != Quaternion.Euler(0,0,0))
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.localPosition = Vector3.MoveTowards(mainCamera.transform.localPosition, previousCameraPos, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }
            pr.stopMovement = false;
        }
        else
        {
            pr.stopMovement = true;
            previousCameraPos = mainCamera.transform.localPosition;
            while (mainCamera.transform.position != inChestCameraTransform.position || mainCamera.transform.localRotation != inChestCameraTransform.localRotation)
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, inChestCameraTransform.position, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, inChestCameraTransform.localRotation, 4);
            }

        }
    }
}
