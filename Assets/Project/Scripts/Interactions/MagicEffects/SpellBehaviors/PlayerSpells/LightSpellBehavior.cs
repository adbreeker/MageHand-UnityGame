using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpellBehavior : SpellBehavior
{
    private GameObject instantiatedEffect;

    private AudioSource spellRemaining;
    private AudioSource spellBurst;

    private void Start()
    {
        spellRemaining = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_SpellLightRemaining, gameObject, minHearingDistance, maxHearingDistance);
        spellRemaining.loop = true;
        spellRemaining.Play();
    }

    public override void OnThrow()
    {

    }

    public override void OnImpact(GameObject impactTarget) //on impact spawn flash effect
    {
        spellRemaining.Stop();
        instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        spellBurst = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_SpellLightBurst, instantiatedEffect, 8f, 30f);
        spellBurst.Play();

        LightImpactOpenWall lightImpactOpenWall = impactTarget.GetComponent<LightImpactOpenWall>();
        if(lightImpactOpenWall != null) 
        {
            lightImpactOpenWall.LightSpellInteract();
        }

        Destroy(gameObject);
    }
}
