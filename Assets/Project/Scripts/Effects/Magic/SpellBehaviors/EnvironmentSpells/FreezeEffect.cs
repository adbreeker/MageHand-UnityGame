using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    float _duration;
    GameObject _freezeEffectPrefab;

    GameObject _freezeOnPlayer;
    Coroutine _freezeEffect;

    public void ActivateFreezeEffect(float duration, GameObject freezeEffectPrefab)
    {
        _duration = duration;
        _freezeEffectPrefab = freezeEffectPrefab;

        if (GetComponent<PotionSpeedEffect>() != null) { GetComponent<PotionSpeedEffect>().DeactivatePotionEffect(); }

        if (_freezeEffect != null) { DeactivateFreezeEffect(); }

        _freezeEffect = StartCoroutine(FreezeDuration());
    }

    public void DeactivateFreezeEffect()
    {
        StopCoroutine(_freezeEffect);
        PlayerParams.Controllers.playerMovement.movementSpeed = PlayerParams.Variables.playerStartingMovementSpeed;
        PlayerParams.Controllers.playerMovement.rotationSpeed = PlayerParams.Variables.playerStartingRotationSpeed;
        Destroy(_freezeOnPlayer);
        Destroy(this);
    }

    IEnumerator FreezeDuration()
    {
        _freezeOnPlayer = Instantiate(_freezeEffectPrefab, gameObject.transform);
        _freezeOnPlayer.AddComponent<SimulationSpeedScaling>();
        PlayerParams.Controllers.playerMovement.movementSpeed = 0.1f * PlayerParams.Variables.playerStartingMovementSpeed;
        PlayerParams.Controllers.playerMovement.rotationSpeed = 0.1f * PlayerParams.Variables.playerStartingRotationSpeed;

        yield return new WaitForSeconds(_duration);

        PlayerParams.Controllers.playerMovement.movementSpeed = PlayerParams.Variables.playerStartingMovementSpeed;
        PlayerParams.Controllers.playerMovement.rotationSpeed = PlayerParams.Variables.playerStartingRotationSpeed;
        Destroy(_freezeOnPlayer);
        Destroy(this);
    }
}
