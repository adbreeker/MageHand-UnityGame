using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Teleportation setup:")]
    public TeleportationSetup teleportationSetup;

    [Header("Teleportation beneficiaries mask")]
    public LayerMask toTeleport;

    private void OnTriggerEnter(Collider toTeleport)
    {
        Vector3 tpDest = teleportationSetup.GetTpDestination(toTeleport.gameObject);
        float tpRot = teleportationSetup.GetTpRotation(toTeleport.gameObject);

        if (toTeleport.gameObject.tag == "Player") //if player add player teleportation effect
        {
            toTeleport.GetComponent<PlayerMovement>().TeleportTo(tpDest, tpRot, gameObject.GetComponent<ParticleSystem>().main.startColor.color);
        }
        else
        {
            if (toTeleport.transform.parent == null) //else add object teleportation effect
            {
                toTeleport.gameObject.transform.position = tpDest;
                toTeleport.gameObject.transform.rotation = Quaternion.Euler(toTeleport.gameObject.transform.rotation.eulerAngles.x, tpRot, toTeleport.gameObject.transform.rotation.eulerAngles.z);

                GameObject tpEffect = GameParams.Holders.materialsAndEffectsHolder.GetEffect(MaterialsAndEffectsHolder.Effects.teleportationObject);
                Instantiate(tpEffect, tpDest, Quaternion.identity)
                    .GetComponent<ParticlesColor>().ChangeColorOfEffect(gameObject.GetComponent<ParticleSystem>().main.startColor.color);
            }
        }
    }
}
