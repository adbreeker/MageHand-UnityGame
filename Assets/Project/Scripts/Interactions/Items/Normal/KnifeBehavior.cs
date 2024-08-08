using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehavior : ItemBehavior
{
    public override void OnPickUp() //fix knife rotation on pick up
    {
        base.OnPickUp();
        transform.localRotation = Quaternion.Euler(120, 0, -90);
    }

    public override void OnThrow()
    {
        GetComponent<Rigidbody>().AddTorque(transform.up * 100);
    }
}
