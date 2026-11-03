using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBarrierBehavior : MonoBehaviour
{
    [SerializeField] GameObject _impactEffectPrefab;

    [Header("Ground barrier:")]
    public bool groundBarrier = false;
    [SerializeField] List<GameObject> _groundBarrierElements = new List<GameObject>();

    float _barrierRadius;

    void Start()
    {
        _barrierRadius = GetComponent<SphereCollider>().radius * transform.lossyScale.x;
        SpawnGroundEffects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnGroundEffects()
    {
        List<Ray> rays = new List<Ray> 
        { 
            new Ray(transform.position, transform.up) ,
            new Ray(transform.position, -transform.up) ,
            new Ray(transform.position, transform.forward) ,
            new Ray(transform.position, -transform.forward) ,
            new Ray(transform.position, transform.right) ,
            new Ray(transform.position, -transform.right)
        };

        foreach (Ray ray in rays) 
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, _barrierRadius);
            foreach (RaycastHit hit in hits) 
            {
                if(hit.collider.CompareTag("Wall") || hit.collider.CompareTag("DungeonCube") || hit.collider.CompareTag("TunnelCube"))
                {
                    foreach(GameObject groundPrefab in _groundBarrierElements)
                    {
                        GameObject newGroundElement = Instantiate(groundPrefab, hit.point, Quaternion.LookRotation(-ray.direction) * Quaternion.Euler(90, 0, 0));
                        newGroundElement.transform.parent = transform;
                        newGroundElement.transform.localScale = (Vector3.one / transform.lossyScale.x) * Mathf.Sqrt(Mathf.Abs(_barrierRadius * _barrierRadius - Vector3.Distance(hit.point, transform.position) * Vector3.Distance(hit.point, transform.position)));
                        newGroundElement.SetActive(true);
                    }
                    break;
                }
            }
        }
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
