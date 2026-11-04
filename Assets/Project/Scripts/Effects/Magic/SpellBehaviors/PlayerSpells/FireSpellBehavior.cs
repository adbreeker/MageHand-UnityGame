using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpellBehavior : SpellBehavior
{
    private GameObject instantiatedEffect;

    private EventInstance spellRemainingSFX;

    private void Start()
    {
        spellRemainingSFX = GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_FireSpellRemaining, transform);
    }

    private void OnDestroy()
    {
        spellRemainingSFX.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public override void OnThrow() //rotate fireball on throw to face good direction
    {
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    public override void OnImpact(GameObject impactTarget) //spawn explosion on impact
    {
        instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_FireSpellBurst, instantiatedEffect.transform);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (Collider collider in colliders) 
        {
            base.OnImpact(collider.gameObject);
        }

        Destroy(gameObject);
    }
}
