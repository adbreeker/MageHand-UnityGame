using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManaRegenBehavior : MonoBehaviour
{
    [Header("Potion duration time")]
    public float duration = 60;

    Coroutine _potionEffect;

    public void OnPickUp()
    {

    }

    public void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        PotionManaRegenBehavior pmb;
        if (PlayerParams.Objects.player.GetComponent<PotionManaRegenBehavior>() != null)
        {
            pmb = PlayerParams.Objects.player.GetComponent<PotionManaRegenBehavior>();
        }
        else
        {
            pmb = PlayerParams.Objects.player.AddComponent<PotionManaRegenBehavior>();
        }

        pmb.duration = duration;

        //active potion effect on player
        pmb.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public void ActivatePotionEffect()
    {
        if (_potionEffect != null) 
        { 
            StopCoroutine(_potionEffect);
            PlayerParams.Controllers.spellCasting.manaRegen = PlayerParams.Variables.startingManaRegen;
        }
        _potionEffect = StartCoroutine(PotionDuration());
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        PlayerParams.Controllers.spellCasting.manaRegen = 5 * PlayerParams.Variables.startingManaRegen;

        yield return new WaitForSeconds(duration);

        PlayerParams.Controllers.spellCasting.manaRegen = PlayerParams.Variables.startingManaRegen;
        Destroy(this);
    }
}
