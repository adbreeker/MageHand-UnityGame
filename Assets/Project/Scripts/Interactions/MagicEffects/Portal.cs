using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Teleportation destination:")]
    public Vector3 teleportationDestination;
    public bool teleportOnCurrentHeight = true;

    [Header("Teleportation rotation:")]
    public float teleportationRotation;
    public bool teleportOnCurrentRotation = false;

    [Header("Teleportation beneficiaries mask")]
    public LayerMask toTeleport;

    private void OnTriggerEnter(Collider toTeleport)
    {
        Vector3 tpDest = teleportationDestination;
        float tpRot = teleportationRotation;
        if (teleportOnCurrentHeight) { tpDest.y = toTeleport.transform.position.y; }
        if (teleportOnCurrentRotation) { tpRot = toTeleport.transform.rotation.eulerAngles.y; }

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
                Instantiate(tpEffect, teleportationDestination, Quaternion.identity)
                    .GetComponent<ParticlesColor>().ChangeColorOfEffect(gameObject.GetComponent<ParticleSystem>().main.startColor.color);
            }
        }
    }
}
