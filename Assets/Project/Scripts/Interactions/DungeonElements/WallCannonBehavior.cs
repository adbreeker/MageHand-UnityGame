using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCannonBehavior : MonoBehaviour
{
    public enum MissileType
    {
        teleportingMissile
    }

    [Header("Launching settings:")]
    public MissileType missileType;
    public GameObject[] missilesPrefabs;
    public float launchingDeley;
    [SerializeField] bool _isLaunching = true;

    [Header("Additional launch settings:")]
    [Header("Teleporting missile")]
    public Vector3 tpDestination;
    public float tpRotation;
    public bool tpOnCurrentRotation = true;

    Vector3 _launchingPos;

    private void Start()
    {
        _launchingPos = transform.position + transform.up * 0.5f;
        if (_isLaunching) { StartCoroutine(LaunchingCoroutine()); }
    }

    IEnumerator LaunchingCoroutine()
    {
        while (true) 
        {
            yield return new WaitForSeconds(launchingDeley);
            LaunchMissile();
        }
    }

    public void SetLaunching(bool setLaunching)
    {
        if (setLaunching)
        {
            _isLaunching = true;
            StopAllCoroutines();
            StartCoroutine(LaunchingCoroutine());
        }
        else
        {
            _isLaunching = false;
            StopAllCoroutines();
        }
    }

    public bool IsLaunching()
    {
        return _isLaunching;
    }

    public void LaunchMissile()
    {
        switch (missileType)
        {
            case MissileType.teleportingMissile:
                LaunchTeleportingMissile();
                break;
        }
    }

    public void LaunchTeleportingMissile()
    {
        Quaternion _launchingRot = transform.rotation * Quaternion.Euler(180.0f, 0, 0);
        GameObject missile = Instantiate(missilesPrefabs[(int)missileType], _launchingPos, _launchingRot);

        TeleportingMissileBehavior missileBehavior = missile.GetComponent<TeleportingMissileBehavior>();
        missileBehavior.teleportationDestination = tpDestination;
        if(!tpOnCurrentRotation)
        {
            missileBehavior.teleportOnCurrentRotation = false;
            missileBehavior.teleportationRotation = tpRotation;
        }

        Rigidbody rb = missile.GetComponent<Rigidbody>();
        rb.AddForce(gameObject.transform.up * 10, ForceMode.Impulse);
    }
}
