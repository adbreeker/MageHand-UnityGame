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
        else if (impactTarget.GetComponent<ItemBehavior>() != null)  //item
        {
            ManageFreezingItem(impactTarget);
        }

        Destroy(gameObject);
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

    private void OnCollisionEnter(Collision collision)
    {
        if(!(collision.gameObject.layer == LayerMask.NameToLayer("Spell") && collision.gameObject.GetComponent<MagicBarrierBehavior>() == null))
        {
            OnImpact(collision.gameObject);
        }
    }
}
