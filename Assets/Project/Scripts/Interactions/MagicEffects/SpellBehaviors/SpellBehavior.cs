using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBehavior : MonoBehaviour
{
    [Header("Special effect")]
    public GameObject specialEffectPrefab;

    public virtual void OnThrow() //rotate fireball on throw to face good direction
    {

    }

    public virtual void OnImpact(GameObject impactTarget) //spawn explosion on impact
    {
        MagicImpactTarget mit = impactTarget.GetComponent<MagicImpactTarget>();
        if (mit != null) { mit.ImpactInteraction(gameObject); }
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
        GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_Teleport, transform.position);
        GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_Teleport, transform);

        transform.position = tpDestination;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, tpRotation, transform.rotation.eulerAngles.z);

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
