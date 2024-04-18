using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonshineBehavior : MonoBehaviour
{
    [Header("Potion duration time")]
    public float duration = 60;
    public int hoochOnPlayer = 1;

    public GameObject drunkRoom;

    Coroutine _hoochEffect;

    public void OnPickUp()
    {

    }

    public void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        PlayerParams.Controllers.spellCasting.mana = 100;

        //find player and add this component
        MoonshineBehavior mb;
        if (PlayerParams.Objects.player.GetComponent<MoonshineBehavior>() != null )
        {
            mb = PlayerParams.Objects.player.GetComponent<MoonshineBehavior>();
            mb.hoochOnPlayer += 1;
        }
        else
        {
            mb = PlayerParams.Objects.player.AddComponent<MoonshineBehavior>();
            mb.drunkRoom = drunkRoom;
        }
        

        mb.duration = duration;

        //active potion effect on player
        mb.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public void ActivatePotionEffect()
    {
        if( hoochOnPlayer >= 3 )
        {
            Instantiate(drunkRoom, new Vector3(Random.Range(1.0f, 5.0f) * 1000, 0, Random.Range(1.0f, 5.0f) * 1000), Quaternion.Euler(0, Random.Range(0,4) * 90, 0));
            if (_hoochEffect != null) { StopCoroutine(_hoochEffect); }
            Destroy(this);
        }
        else
        {
            if (_hoochEffect != null) { StopCoroutine(_hoochEffect); }
            _hoochEffect = StartCoroutine(PotionDuration());
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
