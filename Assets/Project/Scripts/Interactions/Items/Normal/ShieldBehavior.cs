using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : ItemBehavior
{
    public override void OnPickUp() //fix shield rotation on pick up
    {
        base.OnPickUp();
        Vector3 front = PlayerParams.Objects.player.transform.forward * 1000;
        transform.LookAt(front);
    }
}
