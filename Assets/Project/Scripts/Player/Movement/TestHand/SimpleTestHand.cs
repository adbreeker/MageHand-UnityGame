using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Reflection;
using UnityEngine;

public class SimpleTestHand : MoveHandPoints //relic of old times, before gesture recognition
{
    GetObjectsNearHand pointer;
    float startZ;

    protected override void Start()
    {
        //hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        pointer = GetComponent<GetObjectsNearHand>();

        PlayerParams.Controllers.handInteractions = GetComponent<HandInteractions>();
        PlayerParams.Objects.hand = gameObject;

        startZ = transform.localPosition.z;
    }

    protected override void Update()
    {
        HandMovement();
        HandGesture();
    }

    void HandMovement() //moving hand object with mouse
    {
        // Get the mouse position on the screen
        Vector3 mousePos = Input.mousePosition;

        // Use a consistent z-value for the calculation, matching the desired local z position (0.5)
        float zDistance = PlayerParams.Objects.playerCamera.WorldToScreenPoint(transform.parent.position).z + startZ;

        // Convert mouse position to world position at the correct z distance
        Vector3 worldMousePos = PlayerParams.Objects.playerCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDistance));

        // Convert the world position to local space
        Vector3 localMousePos = transform.parent.InverseTransformPoint(worldMousePos);

        // Set the local position with z fixed at 0.5
        transform.localPosition = new Vector3(localMousePos.x, localMousePos.y, startZ);
    }

    void HandGesture()
    {
        gesture = "None";

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(PlayerParams.Controllers.handInteractions.inHand == null)
            {
                if(pointer.currentlyPointing != null)
                {
                    if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("Interaction") ||
                    (pointer.currentlyPointing.layer == LayerMask.NameToLayer("UI") && PlayerParams.Controllers.spellsMenu.menuOpened))
                    {
                        gesture = "Pointing_Up";
                    }

                    if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("Item") ||
                        (pointer.currentlyPointing.layer == LayerMask.NameToLayer("UI") && PlayerParams.Controllers.inventory.inventoryOpened))
                    {
                        gesture = "Closed_Fist";
                    }
                }
            }
            else
            {
                gesture = "Thumb_Up";
            }
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            if(PlayerParams.Controllers.handInteractions.inHand == null)
            {
                gesture = "Victory";
            }
            else
            {
                gesture = "Thumb_Down";
            }
        }
        else if(Input.GetKeyDown(KeyCode.Mouse2))
        {
            gesture = "ILoveYou";
        }
    }
}
