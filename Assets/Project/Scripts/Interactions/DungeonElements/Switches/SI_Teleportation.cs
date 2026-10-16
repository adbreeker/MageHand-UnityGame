using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SI_Teleportation : SwitchInteraction
{
    [Header("Teleportation Setup")]
    public TeleportationSetup teleportationSetup;

    public override void Interact()
    {
        foreach(GameObject interactedObject in interactedObjects)
        {
            if (interactedObject.tag == "Player")
            {
                Vector3 tpDest = teleportationSetup.GetTpDestination(PlayerParams.Objects.player);
                float tpRot = teleportationSetup.GetTpRotation(PlayerParams.Objects.player);

                PlayerParams.Controllers.playerMovement.TeleportTo(tpDest, tpRot, null);
            }
            else
            {

            }
        }
    }
}
