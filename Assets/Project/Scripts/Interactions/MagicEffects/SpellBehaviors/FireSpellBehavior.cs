using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpellBehavior : MonoBehaviour
{
    [Header("Explosion effect prefab")]
    public GameObject explosionEffectPrefab;

    public void OnThrow() //rotate fireball on throw to face good direction
    {
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    public void OnImpact() //spawn explosion on impact
    {
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
    }
}
