using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpell : MonoBehaviour //script added to spell on throw
{
    Rigidbody rb;
    LayerMask notColliders;

    public void Initialize(GameObject player) //initializing throw
    {
        //invoking OnThrow method on spell object
        gameObject.SendMessage("OnThrow");

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

    

    private void OnCollisionEnter(Collision collision)
    {
        //checking if collision layer is not in notColliders layer mask with use of bitwise operation
        if ((1 << collision.gameObject.layer & notColliders.value) == 0) 
        {
            if(collision.gameObject.GetComponent<SpellImpactInteraction>() != null)
            {
                collision.gameObject.GetComponent<SpellImpactInteraction>().OnSpellImpact(gameObject);
            }
            //do something on impact, then destroy
            gameObject.SendMessage("OnImpact");
            Destroy(gameObject);
        }
    }
}