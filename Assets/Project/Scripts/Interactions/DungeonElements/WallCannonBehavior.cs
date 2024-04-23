using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCannonBehavior : MonoBehaviour
{
    public GameObject missileToLaunch;
    public float launchingDeley;
    public bool isLaunching = true;

    Vector3 _launchingPos;
    Quaternion _launchingRot;

    private void Start()
    {
        _launchingPos = transform.position + transform.up * 0.5f;
        _launchingRot = transform.rotation * Quaternion.Euler(180.0f, 0, 0);
        StartCoroutine(LaunchingCoroutine());
    }

    IEnumerator LaunchingCoroutine()
    {
        while (true) 
        {
            if(isLaunching)
            {
                yield return new WaitForSeconds(launchingDeley);
                GameObject missile = Instantiate(missileToLaunch, _launchingPos, _launchingRot);
                missile.AddComponent<ThrowSpell>().Initialize(gameObject, ~LayerMask.GetMask("TransparentFX"));
            }
        }
    }
}
