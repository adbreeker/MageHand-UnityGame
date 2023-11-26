using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpellBehavior : MonoBehaviour
{
    [Header("Flash effect prefab")]
    public GameObject flashEffectPrefab;

    [Header("Sound distance")]
    public float minHearingDistance = 10.0f;
    public float maxHearingDistance = 30.0f;

    private GameObject instantiatedEffect;

    private AudioSource spellRemaining;
    private AudioSource spellBurst;

    private void Start()
    {
        spellRemaining = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_SpellLightRemaining, gameObject, minHearingDistance, maxHearingDistance);
        spellRemaining.loop = true;
        spellRemaining.Play();
    }

    public void OnThrow()
    {

    }

    public void OnImpact() //on impact spawn flash effect
    {
        spellRemaining.Stop();
        instantiatedEffect = Instantiate(flashEffectPrefab, transform.position, Quaternion.identity);
        spellBurst = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_SpellLightBurst, instantiatedEffect, 8f, 30f);
        spellBurst.Play();
    }
}
