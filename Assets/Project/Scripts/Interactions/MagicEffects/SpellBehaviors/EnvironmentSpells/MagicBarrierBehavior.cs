using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBarrierBehavior : MonoBehaviour
{
    [SerializeField] GameObject _impactEffectPrefab;

    [Header("Ground barrier:")]
    public bool groundBarrier = false;
    [SerializeField] List<GameObject> _groundBarrierElements = new List<GameObject>();

    void Start()
    {
        if(groundBarrier)
        {
            foreach(GameObject element in _groundBarrierElements)
            {
                element.SetActive(true);
            }
        }
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
