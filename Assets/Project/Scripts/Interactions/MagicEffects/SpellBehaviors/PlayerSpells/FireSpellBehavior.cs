using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpellBehavior : SpellBehavior
{
    private GameObject instantiatedEffect;

    private AudioSource spellRemaining;
    private AudioSource spellBurst;

    private void Start()
    {
        spellRemaining = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_FireSpellRemaining, gameObject, minHearingDistance, maxHearingDistance);
        spellRemaining.loop = true;
        spellRemaining.Play();
    }

    public override void OnThrow() //rotate fireball on throw to face good direction
    {
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    public override void OnImpact(GameObject impactTarget) //spawn explosion on impact
    {
        instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        spellBurst = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_FireSpellBurst, instantiatedEffect, 8f, 30f);
        spellBurst.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (Collider collider in colliders) 
        {
            Destroyable destroyable = collider.GetComponent<Destroyable>();
            if(destroyable != null)
            {
                destroyable.SplitMesh();
            }
        }

        Destroy(gameObject);
    }
}
