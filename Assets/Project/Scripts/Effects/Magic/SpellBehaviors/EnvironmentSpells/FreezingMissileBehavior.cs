using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingMissileBehavior : SpellBehavior
{
    public float freezeDuration;
    public GameObject freezeEffectPrefab;

    GameObject _instantiatedEffect;

    void Start()
    {
        
    }

    public override void OnImpact(GameObject impactTarget)
    {
        _instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);

        //managing all freezing
        if (impactTarget.layer == LayerMask.NameToLayer("Player")) //player
        {
            ManageFreezingPlayer();
        }
        else if (impactTarget.GetComponent<ItemBehavior>() != null)  //item
        {
            ManageFreezingItem(impactTarget);
        }
        else if (impactTarget.GetComponent<SpellBehavior>() != null) // spell
        {
            ManageFreezingSpell(impactTarget);
        }

        Destroy(gameObject);
    }

    void ManageFreezingPlayer()
    {
        if(PlayerParams.Objects.player.GetComponent<PotionSpeedEffect>() == null) 
        {
            FreezeEffect fe;
            if (PlayerParams.Objects.player.GetComponent<FreezeEffect>() != null)
            {
                fe = PlayerParams.Objects.player.GetComponent<FreezeEffect>();
            }
            else
            {
                fe = PlayerParams.Objects.player.AddComponent<FreezeEffect>();
            }

            fe.ActivateFreezeEffect(freezeDuration, freezeEffectPrefab);
        }
    }

    void ManageFreezingItem(GameObject item)
    {
        if (item.tag == "Shield")
        {
            //shield is protecting from magic missiles
        }
        else
        {
            if (item.transform.parent == null)
            {

            }
            else if (PlayerParams.Controllers.handInteractions.inHand == item)
            {
                ManageFreezingPlayer();
            }
            else
            {

            }
        }
    }

    void ManageFreezingSpell(GameObject spell)
    {
        if (PlayerParams.Controllers.handInteractions.inHand == spell )
        {
            if(spell.GetComponent<FireSpellBehavior>() != null)
            {
                Destroy(spell);
                PlayerParams.Controllers.handInteractions.inHand = null;
                PlayerParams.Controllers.spellCasting.currentSpell = "None";
            }
            else
            {
                ManageFreezingPlayer();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnImpact(collision.collider.gameObject);
    }
}
