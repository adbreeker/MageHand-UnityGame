using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    Rigidbody rb;

    Vector3 previousPos;
    int posCounter = 0;
    
    public void Initialize(GameObject player)
    {
        this.gameObject.transform.SetParent(null);
        previousPos = transform.position;
        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(player.transform.forward * 15, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if(previousPos == transform.position)
        {
            posCounter++;
            if(posCounter >= 10)
            {
                Destroy(rb);
                Destroy(this);
            }
        }
        else
        {
            posCounter = 0;
            previousPos = transform.position;
        }
    }
}
