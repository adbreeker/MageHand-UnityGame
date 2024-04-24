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
    public bool isLaunching = true;

    [Header("Additional launch settings:")]
    public Vector3 tpDestination;

    Vector3 _launchingPos;

    private void Start()
    {
        _launchingPos = transform.position + transform.up * 0.5f;
        StartCoroutine(LaunchingCoroutine());
    }

    IEnumerator LaunchingCoroutine()
    {
        while (true) 
        {
            yield return new WaitForSeconds(launchingDeley);
            if (isLaunching)
            {
                switch (missileType) 
                {
                    case MissileType.teleportingMissile:
                        LaunchTeleportingMissile();
                        break;
                }
            }
        }
    }

    void LaunchTeleportingMissile()
    {
        Quaternion _launchingRot = transform.rotation * Quaternion.Euler(180.0f, 0, 0);
        GameObject missile = Instantiate(missilesPrefabs[(int)missileType], _launchingPos, _launchingRot);
        missile.GetComponent<TeleportingMissileBehavior>().teleportationDestination = tpDestination;

        Rigidbody rb = missile.GetComponent<Rigidbody>();
        rb.AddForce(gameObject.transform.up * 10, ForceMode.Impulse);
    }
}
