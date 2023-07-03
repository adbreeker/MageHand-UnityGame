using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSpeedBehavior : MonoBehaviour
{
    [Header("Potion duration time")]
    public float duration = 30;

    [Header("Speed aura pefab")]
    public GameObject speedAuraPrefab;

    private GameObject auraOnPlayer;

    public void OnPickUp()
    {

    }

    public void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PotionSpeedBehavior psb = player.AddComponent<PotionSpeedBehavior>();

        //set duration and speed aura prefab in script on player
        psb.duration = duration;
        psb.speedAuraPrefab = speedAuraPrefab;

        //active potion effect on player
        psb.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public void ActivatePotionEffect() //activate potion speed effect on player
    {
        auraOnPlayer = Instantiate(speedAuraPrefab, gameObject.transform);
        GetComponent<PlayerMovement>().movementSpeed *= 2;
        GetComponent<PlayerMovement>().rotationSpeed *= 2;
        StartCoroutine(PotionDuration());
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        yield return new WaitForSeconds(duration);
        GetComponent<PlayerMovement>().movementSpeed /= 2;
        GetComponent<PlayerMovement>().rotationSpeed /= 2;
        Destroy(auraOnPlayer);
        Destroy(this);
    }
}
