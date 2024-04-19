using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionPerceptionEffect : PotionEffect
{
    [Space(20f),Header("Perception mark pefab")]
    public GameObject perceptionMarkPrefab;

    public override void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        PotionPerceptionEffect ppe;
        if (PlayerParams.Objects.player.GetComponent<PotionPerceptionEffect>() != null)
        {
            ppe = PlayerParams.Objects.player.GetComponent<PotionPerceptionEffect>();
        }
        else
        {
            ppe = PlayerParams.Objects.player.AddComponent<PotionPerceptionEffect>();
            ppe.perceptionMarkPrefab = perceptionMarkPrefab;
        }

        ppe.duration = duration;

        //active potion effect on player
        ppe.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public override void ActivatePotionEffect()
    {
        if (_potionEffect != null) 
        {
            StopCoroutine(_potionEffect); 
        }
        _potionEffect = StartCoroutine(PotionDuration());
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        while (duration > 0)
        {
            foreach (Collider collider in Physics.OverlapSphere(PlayerParams.Objects.player.transform.position, 20, LayerMask.GetMask("Item", "Interaction")))
            {
                if(collider.gameObject.GetComponent<PerceptionMark>() != null)
                {
                    collider.gameObject.GetComponent<PerceptionMark>().perceptionTime = 10;
                }
                else
                {
                    collider.gameObject.AddComponent<PerceptionMark>().perceptionMarkPrefab = perceptionMarkPrefab;
                }
            }

            yield return new WaitForSeconds(0.1f);
            duration -= 0.1f;
        }
        Destroy(this);
    }
}
