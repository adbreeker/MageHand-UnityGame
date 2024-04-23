using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpell : MonoBehaviour //script added to spell on throw
{
    Rigidbody rb;
    LayerMask notColliders;

    SpellBehavior sb;

    public void Initialize(GameObject player) //initializing throw
    {
        sb = GetComponent<SpellBehavior>();
        sb.OnThrow();

        //create mask for objects not to collide with
        notColliders = LayerMask.GetMask("Player", "UI", "TransparentFX");

        //clear spell object parent
        gameObject.transform.SetParent(null);

        //add rigidbody and set it values
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(player.transform.forward * 10, ForceMode.Impulse);
    }

    public void Initialize(GameObject player, LayerMask specialCollidersMask) //initializing throw for cannons
    {
        sb = GetComponent<SpellBehavior>();
        sb.OnThrow();

        //create mask for objects not to collide with
        notColliders = ~specialCollidersMask;

        //clear spell object parent
        gameObject.transform.SetParent(null);

        //add rigidbody and set it values
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(player.transform.up * 10, ForceMode.Impulse);
    }



    private void OnCollisionEnter(Collision collision)
    {
        //checking if collision layer is not in notColliders layer mask with use of bitwise operation
        if ((1 << collision.gameObject.layer & notColliders.value) == 0) 
        {
            sb.OnImpact(collision.gameObject);
        }
    }
}