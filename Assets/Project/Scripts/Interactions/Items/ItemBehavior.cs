using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemBehavior : InteractableBehavior
{
    [Header("Item parameters:")]
    public string itemName;
    public GameObject itemIcon;
    [SerializeField] private EventReference collisionSound;

    private AudioManager AudioManager => GameParams.Managers.audioManager;
    private FmodEvents FmodEvents => GameParams.Managers.fmodEvents;

    public virtual void OnPickUp()
    {
        Rigidbody  rb = GetComponent<Rigidbody>();
        ThrowObject to = GetComponent<ThrowObject>();

        if(rb != null )
        {
            Destroy(rb);
        }

        if(to != null) 
        {
            Destroy(to);
        }
    }

    public virtual void OnThrow()
    {
        GetComponent<Rigidbody>().AddTorque(transform.right * 100);
    }

    public void TeleportTo(Vector3 tpDestination, float tpRotation) //teleport to destination and stop movement enqued before teleportation
    {
        transform.position = tpDestination;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, tpRotation, transform.rotation.eulerAngles.z);
    }
    public void TeleportTo(Vector3 tpDestination, float tpRotation, Color? tpEffectColor)
    {
        if (PlayerParams.Controllers.inventory.inventory.Contains(gameObject))
        {
            gameObject.SetActive(true);
            PlayerParams.Controllers.inventory.inventory.Remove(gameObject);
            RuntimeManager.PlayOneShotAttached(FmodEvents.SFX_Teleport, PlayerParams.Objects.player);
        }
        else
        {
            AudioManager.PlayOneShotOccluded(FmodEvents.SFX_Teleport, transform.position);
        }
        AudioManager.PlayOneShotOccluded(FmodEvents.SFX_Teleport, transform);

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

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "DungeonCube" || collision.gameObject.tag == "TunnelCube")
        {
            AudioManager.PlayOneShotOccluded(collisionSound, transform);
        }
    }
}
