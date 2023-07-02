using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Vector3 teleportationDestination;
    public LayerMask toTeleport;

    public GameObject teleportationEffect_Player;
    public GameObject teleportationEffect_Object;

    BoxCollider portalCollider;

    private void Start()
    {
        portalCollider = GetComponent<BoxCollider>();
        teleportationDestination.y = 1;
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapBox(portalCollider.bounds.center, portalCollider.bounds.extents, Quaternion.identity, toTeleport);
        foreach(Collider toTeleport in colliders)
        {
            if(toTeleport.gameObject.tag == "Player")
            {
                toTeleport.GetComponent<PlayerMovement>().TeleportTo(teleportationDestination);
                Instantiate(teleportationEffect_Player, toTeleport.gameObject.transform)
                    .GetComponent<TeleportationColor>().ChangeColorOfEffect(gameObject.GetComponent<ParticleSystem>().startColor);
            }
            else
            {
                if(toTeleport.transform.parent == null)
                {
                    toTeleport.gameObject.transform.position = teleportationDestination;
                    Instantiate(teleportationEffect_Object, teleportationDestination, Quaternion.identity)
                        .GetComponent<TeleportationColor>().ChangeColorOfEffect(gameObject.GetComponent<ParticleSystem>().startColor);
                }
            }
        }
    }
}
