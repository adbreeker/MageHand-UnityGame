using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpell : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider c;

    public void Initialize(GameObject player)
    {
        this.gameObject.transform.SetParent(null);

        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.AddForce(player.transform.forward * 10, ForceMode.Impulse);
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f, 1);
        foreach (Collider collider in colliders)
        {
            if(collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                Destroy(gameObject);
            }
        }
    }
}