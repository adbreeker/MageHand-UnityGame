using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour //script added to object on throw
{
    Rigidbody rb;
    
    public void Initialize(GameObject player) //initializing throw
    {
        //clear object parent and get first previous position
        gameObject.transform.SetParent(null);

        //add rigidbody and set it values
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(player.transform.forward * 15, ForceMode.Impulse);
        rb.AddTorque(transform.right * 100);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Switch"))
        {
            collision.gameObject.SendMessage("OnClick");
        }
    }
}
