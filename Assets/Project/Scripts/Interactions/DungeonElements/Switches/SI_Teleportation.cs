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
            if (teleportOnCurrentHeight)
            {
                PlayerParams.Controllers.playerMovement.TeleportTo(
                    new Vector3(tpDestination.x, PlayerParams.Objects.player.transform.position.y, tpDestination.z), null);
            }
            else
            {
                PlayerParams.Controllers.playerMovement.TeleportTo(
                new Vector3(tpDestination.x, tpDestination.y, tpDestination.z), null);
            }

            if (!teleportWithCurrentRotation)
            {
                PlayerParams.Objects.player.transform.rotation = Quaternion.Euler(0, tpRotation, 0);
            }
        }
        else
        {

        }

    }
}
