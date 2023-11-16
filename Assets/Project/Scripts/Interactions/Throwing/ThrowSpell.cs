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
        rb.AddForce(player.transform.forward * 10, ForceMode.Impulse);
    }

    private void Update() //check for collisions
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach (Collider collider in colliders)
        {
            if (!IsLayerInLayerMask(collider.gameObject.layer, notColliders)) //check if spell is colliding with something not on specified layers
            {
                //Invoke OnImpact method on spell object and then destroy object
                gameObject.SendMessage("OnImpact");
                Destroy(gameObject);
                break;
            }
        }

    }

    bool IsLayerInLayerMask(int layer, LayerMask layerMask)
    {
        // Convert the layerMask to a bit mask
        int layerMaskValue = 1 << layer;

        // Check if the layer is in the layerMask
        return (layerMask.value & layerMaskValue) != 0;
    }
}