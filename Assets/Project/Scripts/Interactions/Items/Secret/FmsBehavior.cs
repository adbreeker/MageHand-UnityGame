using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmsBehavior : ItemBehavior
{
    public override void OnPickUp()
    {
        base.OnPickUp();
        transform.localRotation = Quaternion.Euler(180f, -90f, 0f);
    }

    public override void OnThrow()
    {
        GetComponent<Rigidbody>().AddTorque(transform.forward * 100);
    }
}
