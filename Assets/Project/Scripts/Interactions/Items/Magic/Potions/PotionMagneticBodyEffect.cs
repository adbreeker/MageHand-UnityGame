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
                    if (collider.gameObject.GetComponent<ThrowObject>() != null || 
                        IsObjectNotBlocked(collider, LayerMask.GetMask("Default", "Spell", "Interaction", "Item")))
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

    bool IsObjectNotBlocked(Collider collider, LayerMask blockingMask)
    {
        MeshRenderer objectRenderer = collider.GetComponentInChildren<MeshRenderer>();

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(PlayerParams.Objects.playerCamera);
        if (!GeometryUtility.TestPlanesAABB(planes, objectRenderer.bounds))
        {
            return false;
        }

        // Perform detailed visibility check using raycasts
        Bounds bounds = objectRenderer.bounds;
        Vector3 centerPoint = bounds.center;

        // Calculate adjusted corner points
        Vector3[] checkPoints = new Vector3[17];
        checkPoints[0] = centerPoint;

        Vector3[] corners = new Vector3[8]
        {
            bounds.min,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            bounds.max
        };

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 direction = (corners[i] - centerPoint).normalized;
            checkPoints[i + 1] = centerPoint + direction * (bounds.extents.magnitude * 0.5f);
        }
        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 direction = (corners[i] - centerPoint).normalized;
            checkPoints[i + 9] = centerPoint + direction * (bounds.extents.magnitude * 0.9f);
        }

        //foreach (Vector3 point in checkPoints)
        //{
        //    Ray ray = new(PlayerParams.Objects.playerCamera.transform.position, point - PlayerParams.Objects.playerCamera.transform.position);
        //    Debug.DrawRay(ray.origin, ray.direction * 5.5f, Color.red);
        //}

        foreach (Vector3 point in checkPoints)
        {
            Ray ray = new(PlayerParams.Objects.playerCamera.transform.position, point - PlayerParams.Objects.playerCamera.transform.position);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Vector3.Distance(ray.origin, point), blockingMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject == collider.gameObject)
                {
                    //Debug.DrawRay(ray.origin, ray.direction * 5.5f, Color.green);
                    return true; // At least one point is visible
                }
            }
        }

        return false; // None of the points are visible
    }
}
