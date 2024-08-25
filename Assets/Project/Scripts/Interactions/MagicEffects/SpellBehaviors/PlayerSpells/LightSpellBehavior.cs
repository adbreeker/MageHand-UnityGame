using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LightSpellBehavior : SpellBehavior
{
    private GameObject instantiatedEffect;

    private AudioSource spellBurst;
    private EventInstance spellRemainingSound;

    private void Start()
    {
        spellRemainingSound = GameParams.Managers.audioManager.CreateSpatializedAudio(GameParams.Managers.fmodEvents.SFX_SpellLightRemaining, transform, true);
        spellRemainingSound.start();
        spellRemainingSound.release();
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
        spellBurst = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_SpellLightBurst, instantiatedEffect, 8f, 30f);
        spellBurst.Play();

        if(impactTarget != null) 
        {
            LightImpactOpenWall lightImpactOpenWall = impactTarget.GetComponent<LightImpactOpenWall>();
            if (lightImpactOpenWall != null)
            {
                lightImpactOpenWall.LightSpellInteract();
            }
        }

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
