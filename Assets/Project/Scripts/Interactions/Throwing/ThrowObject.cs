using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour //script added to object on throw
{
    Rigidbody rb;
    
    public void Initialize(Vector3 throwDirection) //initializing throw
    {
        //clear object parent and get first previous position
        gameObject.transform.SetParent(null);

        //add rigidbody and set it values
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(throwDirection * 15, ForceMode.Impulse);

        gameObject.SendMessage("OnThrow");
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Interaction"))
        {
            collision.gameObject.SendMessage("OnClick");
        }
    }
}
