using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBarrierBehavior : MonoBehaviour
{
    [SerializeField] GameObject _impactEffectPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(_impactEffectPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].point - gameObject.transform.position), gameObject.transform);

        if(collision.collider.GetComponent<ItemBehavior>() != null)
        {
            Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
            rb.AddExplosionForce(5.0f, collision.contacts[0].point, 1.0f, 3.0f, ForceMode.Impulse);
        }
    }
}
