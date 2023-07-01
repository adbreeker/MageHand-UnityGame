using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpell : MonoBehaviour
{
    Rigidbody rb;
    LayerMask notColliders;

    public void Initialize(GameObject player)
    {
        gameObject.transform.SetParent(null);

        notColliders = LayerMask.GetMask("Player", "UI", "TransparentFX");

        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.AddForce(player.transform.forward * 10, ForceMode.Impulse);
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f, 1);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer != notColliders)
            {
                Destroy(gameObject);
                break;
            }
        }
    }
}