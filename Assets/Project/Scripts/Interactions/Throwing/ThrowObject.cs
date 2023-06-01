using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    Rigidbody rb;

    float previousY;
    
    public void Initialize(GameObject player)
    {
        this.gameObject.transform.SetParent(null);
        previousY = transform.position.y;
        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(player.transform.forward * 15, ForceMode.Impulse);
    }

    private void Update()
    {
        if(previousY < transform.position.y)
        {
            Destroy(rb);
            Destroy(this);
        }
        else
        {
            previousY = transform.position.y;
        }
    }
}
