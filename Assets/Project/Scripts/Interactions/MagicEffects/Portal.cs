using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Teleportation destination")]
    public Vector3 teleportationDestination;
    public bool teleportOnCurrentHeight = true;

    [Header("Teleportation beneficiaries mask")]
    public LayerMask toTeleport;

    [Header("Teleportation effects for objects")]
    public GameObject teleportationEffect_Object;

    BoxCollider portalCollider;

    private void Awake() //get teleport collider and set teleportation destination y variable to 1
    {
        portalCollider = GetComponent<BoxCollider>();
        teleportationDestination.y = 1;
    }

    private void Update()
    {
        //get colliders on specified layers inside portal collider
        Collider[] colliders = Physics.OverlapBox(portalCollider.bounds.center, portalCollider.bounds.extents, Quaternion.identity, toTeleport);

        //teleport all found objects
        foreach(Collider toTeleport in colliders)
        {
            if(toTeleport.gameObject.tag == "Player") //if player add player teleportation effect
            {
                if (teleportOnCurrentHeight)
                {
                    Vector3 nativeHeightDestination = teleportationDestination;
                    nativeHeightDestination.y = toTeleport.transform.position.y;
                    toTeleport.GetComponent<PlayerMovement>().TeleportTo(nativeHeightDestination, gameObject.GetComponent<ParticleSystem>().main.startColor.color);
                }
                else
                {
                    toTeleport.GetComponent<PlayerMovement>().TeleportTo(teleportationDestination, gameObject.GetComponent<ParticleSystem>().main.startColor.color);
                }
            }
            else
            {
                if(toTeleport.transform.parent == null) //else add object teleportation effect
                {
                    if(teleportOnCurrentHeight)
                    {
                        Vector3 nativeHeightDestination = teleportationDestination;
                        nativeHeightDestination.y = toTeleport.transform.position.y;
                        toTeleport.gameObject.transform.position = nativeHeightDestination;
                    }
                    else
                    {
                        toTeleport.gameObject.transform.position = teleportationDestination;
                    }

                    Instantiate(teleportationEffect_Object, teleportationDestination, Quaternion.identity)
                        .GetComponent<TeleportationColor>().ChangeColorOfEffect(gameObject.GetComponent<ParticleSystem>().main.startColor.color);
                }
            }
        }
    }
}
