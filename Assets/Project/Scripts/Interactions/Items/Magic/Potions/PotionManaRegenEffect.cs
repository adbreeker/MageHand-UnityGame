using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManaRegenEffect : PotionEffect
{
    public override void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        PotionManaRegenEffect pmre;
        if (PlayerParams.Objects.player.GetComponent<PotionManaRegenEffect>() != null)
        {
            pmre = PlayerParams.Objects.player.GetComponent<PotionManaRegenEffect>();
        }
        else
        {
            pmre = PlayerParams.Objects.player.AddComponent<PotionManaRegenEffect>();
        }

        pmre.duration = duration;

        //active potion effect on player
        pmre.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public override void ActivatePotionEffect()
    {
        if (_potionEffect != null) 
        { 
            StopCoroutine(_potionEffect);
            PlayerParams.Controllers.spellCasting.manaRegen = PlayerParams.Variables.startingManaRegen;
        }
        _potionEffect = StartCoroutine(PotionDuration());
    }

    public override void DeactivatePotionEffect()
    {
        StopCoroutine(_potionEffect);
        PlayerParams.Controllers.spellCasting.manaRegen = PlayerParams.Variables.startingManaRegen;
        Destroy(this);
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        PlayerParams.Controllers.spellCasting.manaRegen = 5 * PlayerParams.Variables.startingManaRegen;

        yield return new WaitForSeconds(duration);

        PlayerParams.Controllers.spellCasting.manaRegen = PlayerParams.Variables.startingManaRegen;
        Destroy(this);
    }
}
