using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingMissileBehavior : SpellBehavior
{
    [Header("Teleportation Setup")]
    public TeleportationSetup teleportationSetup;

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

    public override void OnImpact(GameObject impactTarget) //spawn explosion on impact
    {
        instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_Teleport, instantiatedEffect.transform);

        
        //managing all teleportations
        if (impactTarget.layer == LayerMask.NameToLayer("Player")) //player
        {
            ManageTeleportingPlayer();
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

    void ManageTeleportingPlayer()
    {
        Vector3 tpDestination = teleportationSetup.GetTpDestination(PlayerParams.Objects.player);
        float tpRotation = teleportationSetup.GetTpRotation(PlayerParams.Objects.player);
        PlayerParams.Controllers.playerMovement.TeleportTo(tpDestination, tpRotation, null);
    }

    void ManageTeleportingItem(GameObject item)
    {
        Vector3 tpDestination = teleportationSetup.GetTpDestination(item);
        float tpRotation = teleportationSetup.GetTpRotation(item);

        if (item.tag == "Shield")
        {
            // shield is protecting from magic missiles
        }
        else
        {
            if (item.transform.parent == null)
            {
                item.GetComponent<ItemBehavior>().TeleportTo(tpDestination, tpRotation, null);
            }
            else if (PlayerParams.Controllers.handInteractions.inHand == item)
            {
                ManageTeleportingPlayer();
            }
            else
            {
                Debug.Log("Teleporting very strange item object");
                item.transform.parent = null;
                item.GetComponent<ItemBehavior>().TeleportTo(tpDestination, tpRotation, null);
            }
        }
    }

    void ManageTeleportingSpell(GameObject spell)
    {
        Vector3 tpDestination = teleportationSetup.GetTpDestination(spell);
        float tpRotation = teleportationSetup.GetTpRotation(spell);

        if (spell.transform.parent == null)
        {
            spell.GetComponent<SpellBehavior>().TeleportTo(tpDestination, tpRotation, null);
        }
        else if (PlayerParams.Controllers.handInteractions.inHand == spell)
        {
            ManageTeleportingPlayer();
        }
        else
        {
            Debug.Log("Teleporting very strange spell object");
            spell.transform.parent = null;
            spell.GetComponent<SpellBehavior>().TeleportTo(tpDestination, tpRotation, null);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        OnImpact(collision.collider.gameObject);
    }
}
