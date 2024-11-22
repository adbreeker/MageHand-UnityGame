using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SimpleTestHand : MoveHandPoints //relic of old times, before gesture recognition
{
    GetObjectsNearHand pointer;
    protected override void Start()
    {
        //hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        pointer = GetComponent<GetObjectsNearHand>();
    }

    protected override void Update()
    {
        HandMovement();
        HandGesture();
    }

    void HandMovement() //moving hand object with mouse
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 handPos = PlayerParams.Objects.playerCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z));

        transform.position = handPos;
    }

    void HandGesture()
    {
        gesture = "None";

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(PlayerParams.Controllers.handInteractions.inHand == null)
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
