using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    Rigidbody rb;
    
    public void Initialize(GameObject player)
    {
        this.gameObject.transform.SetParent(null);
        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(player.transform.forward * 15, ForceMode.Impulse);
    }
}
