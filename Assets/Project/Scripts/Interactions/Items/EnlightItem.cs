using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnlightItem : MonoBehaviour
{
    public Material enlighten;
    Material[] previousMaterials;
    public int enlightenTime = 10;

    MeshRenderer mr;

    void Start()
    {
        enlighten = FindObjectOfType<MaterialHolder>().enlightenObject;
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
    void Update()
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
