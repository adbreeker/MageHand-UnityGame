using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SI_WallCannon : SwitchInteraction
{
    [Header("Interaction options:")]
    public bool launchOnce = false;
    public bool changeLaunchingStatus = false;

    public override void Interact()
    {
        foreach(GameObject interactedObject in interactedObjects)
        {
            WallCannonBehavior wallCannon = interactedObject.GetComponent<WallCannonBehavior>();

            if (launchOnce)
            {
                wallCannon.LaunchMissile();
            }
            if (changeLaunchingStatus)
            {
                if (wallCannon.IsLaunching()) { wallCannon.SetLaunching(false); }
                else { wallCannon.SetLaunching(true); }
            }
        }
    }
}
