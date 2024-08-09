using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : InteractableBehavior
{
    [Header("Item parameters:")]
    public string itemName;
    public GameObject itemIcon;

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

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "DungeonCube")
        {
            int random = Random.Range(1, 4);

            AudioSource collisionSound;

            if (random == 1) collisionSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_Collision1, gameObject, maxHearingDistance: 15f);
            else if (random == 2) collisionSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_Collision2, gameObject, maxHearingDistance: 15f);
            else collisionSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_Collision3, gameObject, maxHearingDistance: 15f);

            collisionSound.Play();
            Destroy(collisionSound, collisionSound.clip.length);
        }
    }
}
