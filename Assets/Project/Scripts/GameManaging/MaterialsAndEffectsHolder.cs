using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsAndEffectsHolder : MonoBehaviour
{
    public enum Materials
    {
        enlightenItem,
        enlightenInteraction
    }

    public enum Effects
    {
        teleportationObject
    }

    [Header ("Materials:")]
    public Material enlightenItem; //material for pointed items (enlightening them)
    public Material enlightenInteraction; //material for pointed interactions (enlightening them)

    [Header("Effects:")]
    public GameObject teleportationObjectEffect;

    public Material GetMaterial(Materials material)
    {
        if(material == Materials.enlightenItem)
        {
            return enlightenItem;
        }
        if(material == Materials.enlightenInteraction)
        {
            return enlightenInteraction;
        }

        return null;
    }

    public GameObject GetEffect(Effects effect) 
    {
        if(effect == Effects.teleportationObject)
        {
            return teleportationObjectEffect;
        }

        return null;
    }
}
