using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingBoltBehavior : SpellBehavior
{
    public Vector3 teleportationDestination;

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
        //transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    public override void OnImpact(GameObject impactTarget) //spawn explosion on impact
    {
        instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        spellBurst = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_MagicalTeleportation, instantiatedEffect, 8f, 30f);
        spellBurst.Play();

        teleportationDestination.y = impactTarget.transform.position.y;

        if (impactTarget.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerParams.Controllers.playerMovement.TeleportTo(teleportationDestination, null);
        }
        else if(impactTarget.GetComponent<ItemBehavior>() != null) 
        {
            impactTarget.GetComponent<ItemBehavior>().TeleportTo(teleportationDestination, null);
        }

        Destroy(gameObject);
    }
}
