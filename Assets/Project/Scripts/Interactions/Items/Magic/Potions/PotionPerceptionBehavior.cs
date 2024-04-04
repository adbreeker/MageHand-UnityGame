using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionPerceptionBehavior : MonoBehaviour
{
    [Header("Potion duration time")]
    public float duration = 90.0f;

    [Header("Perception mark pefab")]
    public GameObject perceptionMarkPrefab;

    Coroutine _potionEffect;

    public void OnPickUp()
    {

    }

    public void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        PotionPerceptionBehavior ppb;
        if (PlayerParams.Objects.player.GetComponent<PotionPerceptionBehavior>() != null)
        {
            ppb = PlayerParams.Objects.player.GetComponent<PotionPerceptionBehavior>();
        }
        else
        {
            ppb = PlayerParams.Objects.player.AddComponent<PotionPerceptionBehavior>();
            ppb.perceptionMarkPrefab = perceptionMarkPrefab;
        }

        ppb.duration = duration;

        //active potion effect on player
        ppb.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public void ActivatePotionEffect()
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
