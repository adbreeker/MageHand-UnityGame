using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoochBehavior : MonoBehaviour
{
    [Header("Potion duration time")]
    public float duration = 60;
    public int hoochOnPlayer = 1;

    Coroutine hoochEffect;

    public void OnPickUp()
    {

    }

    public void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        GameObject player = PlayerParams.Objects.player;
        HoochBehavior hb;
        if (player.GetComponent<HoochBehavior>() != null )
        {
            hb = player.GetComponent<HoochBehavior>();
            hb.hoochOnPlayer += 1;
        }
        else
        {
            hb = player.AddComponent<HoochBehavior>();
        }
        

        hb.duration = duration;

        //active potion effect on player
        hb.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public void ActivatePotionEffect()
    {
        if( hoochOnPlayer >= 3 )
        {
            Debug.Log("Ty alkoholiku! masz szczescie ze jeszcze tego nie zaimplementowalem");
        }
        else
        {
            if (hoochEffect != null) { StopCoroutine(hoochEffect); }
            hoochEffect = StartCoroutine(PotionDuration());
        }
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
