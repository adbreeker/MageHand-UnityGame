using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingMissileBehavior : SpellBehavior
{
    public Vector3 teleportationDestination;
    public bool teleportOnNativeHeight = true;

    private GameObject instantiatedEffect;

    private AudioSource spellRemaining;
    private AudioSource spellBurst;

    private void Start()
    {
        spellRemaining = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_FireSpellRemaining, gameObject, minHearingDistance, maxHearingDistance);
        spellRemaining.loop = true;
        spellRemaining.Play();
    }

    public override void OnImpact(GameObject impactTarget) //spawn explosion on impact
    {
        instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        spellBurst = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_MagicalTeleportation, instantiatedEffect, 8f, 30f);
        spellBurst.Play();


        //managing all teleportations
        if (impactTarget.layer == LayerMask.NameToLayer("Player")) //player
        {
            if (teleportOnNativeHeight) { teleportationDestination.y = PlayerParams.Objects.player.transform.position.y; }
            PlayerParams.Controllers.playerMovement.TeleportTo(teleportationDestination, null);
        }
        else if(impactTarget.GetComponent<ItemBehavior>() != null)  //item
        {
            ManageTeleportingItem(impactTarget);
        }
        else if(impactTarget.GetComponent<SpellBehavior>() != null) //spell
        {
            ManageTeleportingSpell(impactTarget);
        }

        Destroy(gameObject);
    }

    void ManageTeleportingItem(GameObject item)
    {
        if (item.transform.parent == null)
        {
            if (teleportOnNativeHeight) { teleportationDestination.y = item.transform.position.y; }
            item.GetComponent<ItemBehavior>().TeleportTo(teleportationDestination, null);
        }
        else if (PlayerParams.Controllers.handInteractions.inHand == item)
        {
            if (item.tag == "Shield")
            {
                // shield is protecting from magic missiles
            }
            else
            {
                if (teleportOnNativeHeight) { teleportationDestination.y = PlayerParams.Objects.player.transform.position.y; }
                PlayerParams.Controllers.playerMovement.TeleportTo(teleportationDestination, null);
            }
        }
        else
        {
            if (teleportOnNativeHeight) { teleportationDestination.y = item.transform.position.y; }
            item.transform.parent = null;
            item.GetComponent<ItemBehavior>().TeleportTo(teleportationDestination, null);
        }
    }

    void ManageTeleportingSpell(GameObject spell)
    {
        if (spell.transform.parent == null)
        {
            if (teleportOnNativeHeight) { teleportationDestination.y = spell.transform.position.y; }
            spell.GetComponent<SpellBehavior>().TeleportTo(teleportationDestination, null);
        }
        else if (PlayerParams.Controllers.handInteractions.inHand == spell)
        {
            if (teleportOnNativeHeight) { teleportationDestination.y = PlayerParams.Objects.player.transform.position.y; }
            PlayerParams.Controllers.playerMovement.TeleportTo(teleportationDestination, null);
        }
        else
        {
            if (teleportOnNativeHeight) { teleportationDestination.y = spell.transform.position.y; }
            Debug.Log("Teleporting very strange spell object");
            spell.transform.parent = null;
            spell.GetComponent<SpellBehavior>().TeleportTo(teleportationDestination, null);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        OnImpact(collision.gameObject);
    }
}
