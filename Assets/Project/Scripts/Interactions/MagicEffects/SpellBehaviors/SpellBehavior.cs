using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBehavior : MonoBehaviour
{
    [Header("Special effect")]
    public GameObject specialEffectPrefab;

    [Header("Sound distance:")]
    public float minHearingDistance = 10.0f;
    public float maxHearingDistance = 30.0f;

    public virtual void OnThrow() //rotate fireball on throw to face good direction
    {

    }

    public virtual void OnImpact(GameObject impactTarget) //spawn explosion on impact
    {

    }

    public virtual void Reactivation()
    {

    }

    public void TeleportTo(Vector3 tpDestination, float tpRotation) //teleport to destination and stop movement enqued before teleportation
    {
        transform.position = tpDestination;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, tpRotation, transform.rotation.eulerAngles.z);
    }
    public void TeleportTo(Vector3 tpDestination, float tpRotation, Color? tpEffectColor)
    {
        transform.position = tpDestination;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, tpRotation, transform.rotation.eulerAngles.z);

        AudioSource tpSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_MagicalTeleportation);
        tpSound.Play();
        Destroy(tpSound, tpSound.clip.length);

        GameObject tpEffect = GameParams.Holders.materialsAndEffectsHolder.GetEffect(MaterialsAndEffectsHolder.Effects.teleportationObject);

        if (tpEffectColor != null)
        {
            Instantiate(tpEffect, transform)
                    .GetComponent<ParticlesColor>().ChangeColorOfEffect(tpEffectColor.Value);
        }
        else
        {
            Instantiate(tpEffect, transform);
        }
    }
}
