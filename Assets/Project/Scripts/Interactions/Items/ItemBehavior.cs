using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ItemBehavior : MonoBehaviour
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

    public void TeleportTo(Vector3 tpDestination) //teleport to destination and stop movement enqued before teleportation
    {
        transform.position = tpDestination;
    }
    public void TeleportTo(Vector3 tpDestination, Color? tpEffectColor)
    {
        transform.position = tpDestination;

        AudioSource tpSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_MagicalTeleportation);
        tpSound.Play();
        Destroy(tpSound, tpSound.clip.length);

        GameObject tpEffect = FindObjectOfType<MaterialsAndEffectsHolder>().GetEffect(MaterialsAndEffectsHolder.Effects.teleportationObject);

        if (tpEffectColor != null)
        {
            Instantiate(tpEffect, transform)
                    .GetComponent<TeleportationColor>().ChangeColorOfEffect(tpEffectColor.Value);
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

            if (random == 1) collisionSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Collision1, gameObject, maxHearingDistance: 15f);
            else if (random == 2) collisionSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Collision2, gameObject, maxHearingDistance: 15f);
            else collisionSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Collision3, gameObject, maxHearingDistance: 15f);

            collisionSound.Play();
            Destroy(collisionSound, collisionSound.clip.length);
        }
    }
}
