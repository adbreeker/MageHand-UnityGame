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
        gameObject.transform.SetParent(null);
        previousPos = transform.position;

        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(player.transform.forward * 15, ForceMode.Impulse);
        rb.AddTorque(transform.right * 100);
    }

    private void FixedUpdate()
    {
        if(previousPos == transform.position)
        {
            posCounter++;
            if(posCounter >= 5)
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
