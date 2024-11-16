using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonshinePotionEffect : PotionEffect
{
    [Space(20f), Header("Moonshines effect on player")]
    public int moonshineOnPlayer = 1;

    [Header("Drunk room prefab")]
    public GameObject drunkRoom;


    public override void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        PlayerParams.Controllers.spellCasting.mana = 100;

        //find player and add this component
        MoonshinePotionEffect mpe;
        if (PlayerParams.Objects.player.GetComponent<MoonshinePotionEffect>() != null )
        {
            mpe = PlayerParams.Objects.player.GetComponent<MoonshinePotionEffect>();
            mpe.moonshineOnPlayer += 1;
        }
        else
        {
            mpe = PlayerParams.Objects.player.AddComponent<MoonshinePotionEffect>();
            mpe.drunkRoom = drunkRoom;
        }
        

        mpe.duration = duration;

        //active potion effect on player
        mpe.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public override void ActivatePotionEffect()
    {
        if( moonshineOnPlayer >= 3 )
        {
            Instantiate(drunkRoom, new Vector3(Random.Range(1.0f, 5.0f) * 1000, 0, Random.Range(1.0f, 5.0f) * 1000), Quaternion.Euler(0, Random.Range(0,4) * 90, 0));
            if (_potionEffect != null) { StopCoroutine(_potionEffect); }
            Destroy(this);
        }
        else
        {
            if (_potionEffect != null) { StopCoroutine(_potionEffect); }
            _potionEffect = StartCoroutine(PotionDuration());
        }
    }

    public override void DeactivatePotionEffect()
    {
        StopCoroutine(_potionEffect);
        Destroy(this);
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        while(duration > 0)
        {
            yield return new WaitForSeconds(1.0f);
            duration -= 1;

            PlayerParams.Objects.player.transform.rotation = Quaternion.Euler(0, 90 * Random.Range(1, 4), 0);
        }
        Destroy(this);
    }
}
