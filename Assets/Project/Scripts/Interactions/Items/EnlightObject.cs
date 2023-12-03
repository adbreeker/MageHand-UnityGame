using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnlightObject : MonoBehaviour //component added to object to enlight it
{
    [Header("Enlightening time")]
    public int enlightenTime = 10;

    [Header("Material type")]
    public MaterialHolder.Materials materialType;

    Material enlighten; //enlighten material
    Material[] previousMaterials; //before enlight materials
    
    MeshRenderer mr;

    void Start() //enlightening this mesh or first mesh in children
    {
        enlighten = FindObjectOfType<MaterialHolder>().GetMaterial(materialType);
            
        if(this.GetComponent<MeshRenderer>() != null)
        {
            mr = this.GetComponent<MeshRenderer>();
        }
        else
        {
            mr = this.GetComponentInChildren<MeshRenderer>();
        }
        previousMaterials = mr.materials;


        mr.materials = EnlightenMaterials(mr.materials.Length);
    }

    // Update is called once per frame
    void FixedUpdate() //counting time to stop enlightening
    {
        enlightenTime--;
        if(enlightenTime <= 0)
        {
            mr.materials = previousMaterials;
            DestroyImmediate(this);
        }
    }

    Material[] EnlightenMaterials(int matL)
    {
        Material[] em = new Material[matL];
        for(int i = 0; i < matL; i++)
        {
            em[i] = enlighten;
        }
        return em;
    }
}
