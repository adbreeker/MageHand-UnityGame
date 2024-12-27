using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionMagneticBodyEffect : PotionEffect
{
    [Space(20f), Header("Magnetic fly pefab")]
    public GameObject magneticFlyPrefab;

    public override void Drink() //add potion component to player, activate potion effect and destroy this object
    {
        //find player and add this component
        PotionMagneticBodyEffect pmbe;
        if (PlayerParams.Objects.player.GetComponent<PotionMagneticBodyEffect>() != null)
        {
            pmbe = PlayerParams.Objects.player.GetComponent<PotionMagneticBodyEffect>();
        }
        else
        {
            pmbe = PlayerParams.Objects.player.AddComponent<PotionMagneticBodyEffect>();
            pmbe.magneticFlyPrefab = magneticFlyPrefab;
        }

        pmbe.duration = duration;

        //active potion effect on player
        pmbe.ActivatePotionEffect();

        //destroy this object
        Destroy(gameObject);
    }

    public override void ActivatePotionEffect()
    {
        if (_potionEffect != null)
        {
            StopCoroutine(_potionEffect);
            PlayerParams.Controllers.spellCasting.manaRegen = PlayerParams.Variables.startingManaRegen;
        }
        _potionEffect = StartCoroutine(PotionDuration());
    }

    public override void DeactivatePotionEffect()
    {
        StopCoroutine(_potionEffect);
        PlayerParams.Controllers.spellCasting.manaRegen = PlayerParams.Variables.startingManaRegen;
        Destroy(this);
    }

    IEnumerator PotionDuration() //count potion effect duration
    {
        List<GameObject> affectedItems = new List<GameObject>();
        while (duration > 0)
        {

            foreach (Collider collider in Physics.OverlapSphere(PlayerParams.Objects.player.transform.position, 5f, LayerMask.GetMask("Item")))
            {
                if(collider.GetComponent<InteractableBehavior>().isInteractable && !affectedItems.Contains(collider.gameObject))
                {
                    Ray ray = new Ray(PlayerParams.Objects.player.transform.position, collider.gameObject.transform.position - PlayerParams.Objects.player.transform.position);
                    if (collider.gameObject.GetComponent<ThrowObject>() != null
                        || Physics.Raycast(ray, Vector3.Distance(PlayerParams.Objects.player.transform.position, collider.gameObject.transform.position), LayerMask.GetMask("Default", "Spell", "Interaction"), QueryTriggerInteraction.Ignore) == false)
                    {
                        affectedItems.Add(collider.gameObject);
                        GameObject magneticFlyEffect = Instantiate(magneticFlyPrefab, collider.gameObject.transform);
                        collider.gameObject.AddComponent<MagneticAttraction>().Initialize(PlayerParams.Objects.player, magneticFlyEffect);
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
            duration -= 0.1f;
        }
        Destroy(this);
    }
}
