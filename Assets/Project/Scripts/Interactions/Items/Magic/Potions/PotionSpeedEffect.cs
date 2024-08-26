using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSpeedEffect : PotionEffect
{
    [Space(20f),Header("Speed aura pefab")]
    public GameObject speedAuraPrefab;

    private GameObject auraOnPlayer;


    public override void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        PotionSpeedEffect pse;
        if (PlayerParams.Objects.player.GetComponent<PotionSpeedEffect>() != null)
        {
            pse = PlayerParams.Objects.player.GetComponent<PotionSpeedEffect>();
        }
        else
        {
            pse = PlayerParams.Objects.player.AddComponent<PotionSpeedEffect>();
            pse.speedAuraPrefab = speedAuraPrefab;
        }

        pse.duration = duration;

        //active potion effect on player
        pse.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public override void ActivatePotionEffect() //activate potion speed effect on player
    {
        if(GetComponent<FreezeEffect>() != null) { GetComponent<FreezeEffect>().StopFreezeEffect(); }

        if (_potionEffect != null) { StopPotionEffect(); }

        _potionEffect = StartCoroutine(PotionDuration());
    }

    public void StopPotionEffect()
    {
        StopCoroutine(_potionEffect);
        PlayerParams.Controllers.playerMovement.movementSpeed = PlayerParams.Variables.playerStartingMovementSpeed;
        PlayerParams.Controllers.playerMovement.rotationSpeed = PlayerParams.Variables.playerStartingRotationSpeed;
        Destroy(auraOnPlayer);
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
