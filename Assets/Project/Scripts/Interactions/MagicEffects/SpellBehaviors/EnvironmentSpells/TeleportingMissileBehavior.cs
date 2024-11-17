using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TeleportingMissileBehavior : SpellBehavior
{
    public Vector3 teleportationDestination;
    public bool teleportOnCurrentHeight = true;

    public float teleportationRotation;
    public bool teleportOnCurrentRotation = true;

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

        Vector3 tpDest = teleportationDestination;
        float tpRot = teleportationRotation;
        if(teleportOnCurrentHeight) { tpDest.y = impactTarget.transform.position.y; }
        if(teleportOnCurrentRotation) { tpRot = impactTarget.transform.rotation.eulerAngles.y; }

        //managing all teleportations
        if (impactTarget.layer == LayerMask.NameToLayer("Player")) //player
        {
            ManageTeleportingPlayer(tpDest, tpRot);
        }
        else if(impactTarget.GetComponent<ItemBehavior>() != null)  //item
        {
            ManageTeleportingItem(impactTarget, tpDest, tpRot);
        }
        else if(impactTarget.GetComponent<SpellBehavior>() != null) //spell
        {
            ManageTeleportingSpell(impactTarget, tpDest, tpRot);
        }

        Destroy(gameObject);
    }

    void ManageTeleportingPlayer(Vector3 tpDestination, float tpRotation)
    {
        PlayerParams.Controllers.playerMovement.TeleportTo(tpDestination, tpRotation, null);
    }

    void ManageTeleportingItem(GameObject item, Vector3 tpDestination, float tpRotation)
    {
        if (item.transform.parent == null)
        {
            item.GetComponent<ItemBehavior>().TeleportTo(tpDestination, tpRotation, null);
        }
        else if (PlayerParams.Controllers.handInteractions.inHand == item)
        {
            if (item.tag == "Shield")
            {
                // shield is protecting from magic missiles
            }
            else
            {
                PlayerParams.Controllers.playerMovement.TeleportTo(tpDestination, tpRotation, null);
            }
        }
        else
        {
            item.transform.parent = null;
            item.GetComponent<ItemBehavior>().TeleportTo(tpDestination, tpRotation, null);
        }
    }

    void ManageTeleportingSpell(GameObject spell, Vector3 tpDestination, float tpRotation)
    {
        if (spell.transform.parent == null)
        {
            spell.GetComponent<SpellBehavior>().TeleportTo(tpDestination, tpRotation, null);
        }
        else if (PlayerParams.Controllers.handInteractions.inHand == spell)
        {
            PlayerParams.Controllers.playerMovement.TeleportTo(tpDestination, tpRotation, null);
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
