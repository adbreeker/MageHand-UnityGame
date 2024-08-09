using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SI_Teleportation : SwitchInteraction
{
    [Header("Destination")]
    public Vector3 tpDestination;
    public bool teleportOnCurrentHeight = true;

    [Header("Rotation")]
    public float tpRotation;
    public bool teleportWithCurrentRotation = true;

    public override void Interact()
    {
        if(interactedObject.tag == "Player")
        {
            Vector3 tpDest = tpDestination;
            float tpRot = tpRotation;
            if(teleportOnCurrentHeight) { tpDest.y = PlayerParams.Objects.player.transform.position.y; }
            if(teleportWithCurrentRotation) { tpRot = PlayerParams.Objects.player.transform.rotation.eulerAngles.y; }

            PlayerParams.Controllers.playerMovement.TeleportTo(tpDest, tpRot, null);
        }
        else
        {

        }
    }
}
