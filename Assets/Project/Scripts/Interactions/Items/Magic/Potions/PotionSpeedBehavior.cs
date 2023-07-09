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
        GameObject player = PlayerParams.Objects.player;
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
        PlayerParams.Controllers.playerMovement.movementSpeed = 2*PlayerParams.Variables.playerStartingMovementSpeed;
        PlayerParams.Controllers.playerMovement.rotationSpeed = 2* PlayerParams.Variables.playerStartingRotationSpeed;
        StartCoroutine(PotionDuration());
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        yield return new WaitForSeconds(duration);
        PlayerParams.Controllers.playerMovement.movementSpeed = PlayerParams.Variables.playerStartingMovementSpeed;
        PlayerParams.Controllers.playerMovement.rotationSpeed = PlayerParams.Variables.playerStartingRotationSpeed;
        Destroy(auraOnPlayer);
        Destroy(this);
    }
}
