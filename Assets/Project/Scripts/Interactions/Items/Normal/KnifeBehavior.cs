using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehavior : ItemBehavior
{
    public override void OnPickUp() //fix knife rotation on pick up
    {
        base.OnPickUp();
        Vector3 front = PlayerParams.Objects.player.transform.forward * -1000;
        front.y = -2000;
        transform.LookAt(front);
    }
}
