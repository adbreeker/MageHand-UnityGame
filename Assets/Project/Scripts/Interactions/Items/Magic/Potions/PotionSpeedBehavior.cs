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

    Coroutine _potionEffect;

    public void OnPickUp()
    {

    }

    public void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        PotionSpeedBehavior psb;
        if (PlayerParams.Objects.player.GetComponent<PotionSpeedBehavior>() != null)
        {
            psb = PlayerParams.Objects.player.GetComponent<PotionSpeedBehavior>();
        }
        else
        {
            psb = PlayerParams.Objects.player.AddComponent<PotionSpeedBehavior>();
            psb.speedAuraPrefab = speedAuraPrefab;
        }

        psb.duration = duration;

        //active potion effect on player
        psb.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public void ActivatePotionEffect() //activate potion speed effect on player
    {
        if (_potionEffect != null) 
        { 
            StopCoroutine(_potionEffect);
            PlayerParams.Controllers.playerMovement.movementSpeed = PlayerParams.Variables.playerStartingMovementSpeed;
            PlayerParams.Controllers.playerMovement.rotationSpeed = PlayerParams.Variables.playerStartingRotationSpeed;
            Destroy(auraOnPlayer);
        }
        _potionEffect = StartCoroutine(PotionDuration());
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        auraOnPlayer = Instantiate(speedAuraPrefab, gameObject.transform);
        PlayerParams.Controllers.playerMovement.movementSpeed = 2 * PlayerParams.Variables.playerStartingMovementSpeed;
        PlayerParams.Controllers.playerMovement.rotationSpeed = 2 * PlayerParams.Variables.playerStartingRotationSpeed;

        yield return new WaitForSeconds(duration);

        PlayerParams.Controllers.playerMovement.movementSpeed = PlayerParams.Variables.playerStartingMovementSpeed;
        PlayerParams.Controllers.playerMovement.rotationSpeed = PlayerParams.Variables.playerStartingRotationSpeed;
        Destroy(auraOnPlayer);
        Destroy(this);
    }
}
