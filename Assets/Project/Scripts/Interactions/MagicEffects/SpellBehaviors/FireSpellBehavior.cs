using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpellBehavior : MonoBehaviour
{
    public GameObject explosionEffectPrefab;

    public void OnThrow()
    {
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    public void OnImpact()
    {
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
    }
}
