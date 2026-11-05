using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MagicBarrierBehavior : MonoBehaviour
{
    enum BarrierLaunchType
    {
        Awake,
        Proximity,
        None
    }

    [SerializeField] GameObject _impactEffectPrefab;
    [SerializeField] SphereCollider _barrierCollider;

    [Header("Barrier launch type:")]
    [SerializeField] BarrierLaunchType _barrierLaunchType = BarrierLaunchType.Awake;
    [SerializeField] Collider _proximityCollider;

    [Header("Ground barrier:")]
    [SerializeField] bool _spawnGroundBarrier = true;
    [SerializeField] List<GameObject> _groundBarrierElements = new List<GameObject>();

    [Header("Events:")]
    public UnityEvent OnBarrierActivated;
    public UnityEvent OnBarrierHit;

    float _barrierRadius;
    bool isBarrierActive = false;

    private void Awake()
    {
        _barrierRadius = _barrierCollider.radius * transform.lossyScale.x;
        if (_spawnGroundBarrier) { SpawnGroundEffects(); }

        if (_barrierLaunchType == BarrierLaunchType.Awake) 
        { 
            ActivateBarrier();
        }
        else if (_barrierLaunchType == BarrierLaunchType.Proximity)
        {
            _proximityCollider.enabled = true;
        }
    }

    public void ActivateBarrier()
    {
        if (!isBarrierActive)
        {
            isBarrierActive = true;
            GetComponent<ParticleSystem>().Play(true);
            _proximityCollider.enabled = false;
            OnBarrierActivated?.Invoke();
        }
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
        if(isBarrierActive)
        {
            Instantiate(_impactEffectPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].point - gameObject.transform.position), gameObject.transform);
            OnBarrierHit?.Invoke();

            if (collision.collider.GetComponent<ItemBehavior>() != null)
            {
                Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
                rb.AddExplosionForce(5.0f, collision.contacts[0].point, 1.0f, 3.0f, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_barrierLaunchType == BarrierLaunchType.Proximity)
        {
            ActivateBarrier();
        }
    }
}
