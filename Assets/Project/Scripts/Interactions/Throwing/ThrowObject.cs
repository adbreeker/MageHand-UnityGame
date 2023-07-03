using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour //script added to object on throw
{
    Rigidbody rb;

    //necessary values for counting if object stopped moving
    Vector3 previousPos;
    int posCounter = 0;
    
    public void Initialize(GameObject player) //initializing throw
    {
        //clear object parent and get first previous position
        gameObject.transform.SetParent(null);
        previousPos = transform.position;

        //add rigidbody and set it values
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(player.transform.forward * 15, ForceMode.Impulse);
        rb.AddTorque(transform.right * 100);
    }

    private void FixedUpdate() //check if object still flying
    {
        if(previousPos == transform.position)
        {
            posCounter++;
            if(posCounter >= 5) //if object stopped moving destroy rigidbody and this script
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
