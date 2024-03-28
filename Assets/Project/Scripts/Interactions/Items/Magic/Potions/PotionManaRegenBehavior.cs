using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManaRegenBehavior : MonoBehaviour
{
    [Header("Potion duration time")]
    public float duration = 60;

    public void OnPickUp()
    {

    }

    public void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        GameObject player = PlayerParams.Objects.player;
        PotionManaRegenBehavior psb = player.AddComponent<PotionManaRegenBehavior>();

        psb.duration = duration;

        //active potion effect on player
        psb.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public void ActivatePotionEffect()
    {
        PlayerParams.Controllers.spellCasting.manaRegen = 5 * PlayerParams.Variables.startingManaRegen;
        StartCoroutine(PotionDuration());
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        yield return new WaitForSeconds(duration);
        PlayerParams.Controllers.spellCasting.manaRegen = PlayerParams.Variables.startingManaRegen;
        Destroy(this);
    }
}
