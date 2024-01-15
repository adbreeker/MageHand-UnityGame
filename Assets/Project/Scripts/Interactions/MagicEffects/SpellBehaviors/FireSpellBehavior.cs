using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpellBehavior : MonoBehaviour
{
    [Header("Explosion effect prefab")]
    public GameObject explosionEffectPrefab;

    [Header("Sound distance")]
    public float minHearingDistance = 10.0f;
    public float maxHearingDistance = 30.0f;

    private GameObject instantiatedEffect;

    private AudioSource spellRemaining;
    private AudioSource spellBurst;

    private void Start()
    {
        spellRemaining = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_FireSpellRemaining, gameObject, minHearingDistance, maxHearingDistance);
        spellRemaining.loop = true;
        spellRemaining.Play();
    }

    public void OnThrow() //rotate fireball on throw to face good direction
    {
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    public void OnImpact() //spawn explosion on impact
    {
        instantiatedEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        spellBurst = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_FireSpellBurst, instantiatedEffect, 8f, 30f);
        spellBurst.Play();
    }
}
