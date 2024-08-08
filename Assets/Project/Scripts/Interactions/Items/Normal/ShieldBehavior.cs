using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : ItemBehavior
{
    public override void OnPickUp() //fix shield rotation on pick up
    {
        base.OnPickUp();
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public override void OnThrow()
    {
        
    }
}
