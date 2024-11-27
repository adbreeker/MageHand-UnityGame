using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LightSpellBehavior : SpellBehavior
{
    private GameObject instantiatedEffect;
    private EventInstance spellRemainingSound;

    private void Start()
    {
        spellRemainingSound = GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_LightSpellRemaining, transform);
    }

    private void OnDestroy()
    {
        spellRemainingSound.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public override void OnThrow()
    {

    }

    public override void OnImpact(GameObject impactTarget) //on impact spawn flash effect
    {
        spellRemainingSound.stop(STOP_MODE.IMMEDIATE);
        instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_LightSpellBurst, instantiatedEffect.transform);

        base.OnImpact(impactTarget);

        Destroy(gameObject);
    }

    public override void Reactivation() // create floating light
    {
        gameObject.AddComponent<FloatingLight>();

        if (PlayerParams.Controllers.spellCasting.floatingLight != null) //if floatin light actually exists then replacing it
        {
            Destroy(PlayerParams.Controllers.spellCasting.floatingLight);
        }
        PlayerParams.Controllers.spellCasting.floatingLight = gameObject;
        PlayerParams.Controllers.handInteractions.inHand = null;
        PlayerParams.Controllers.spellCasting.currentSpell = "None";
    }
}
