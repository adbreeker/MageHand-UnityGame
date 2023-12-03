using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialHolder : MonoBehaviour
{
    public enum Materials
    {
        enlightenItem,
        enlightenInteraction
    }

    public Material enlightenItem; //material for pointed items (enlightening them)
    public Material enlightenInteraction; //material for pointed interactions (enlightening them)

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
}
