using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Progress;

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

    void ManageFreezingItem(GameObject item)
    {
        if (item.transform.parent == null)
        {
            
        }
        else if (PlayerParams.Controllers.handInteractions.inHand == item)
        {
            if (item.tag == "Shield")
            {
                //shield is protecting from magic missiles
            }
            else
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
        else
        {

        }
    }

    void ManageFreezingSpell(GameObject spell)
    {
        if (PlayerParams.Controllers.handInteractions.inHand == spell )
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

    private void OnCollisionEnter(Collision collision)
    {
        OnImpact(collision.collider.gameObject);
    }
}
