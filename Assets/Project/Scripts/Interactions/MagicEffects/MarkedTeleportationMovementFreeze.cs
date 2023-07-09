using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkedTeleportationMovementFreeze : MonoBehaviour
{
    private void OnDestroy()
    {
        PlayerParams.Controllers.playerMovement.stopMovement = false;
    }
}
