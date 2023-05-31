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
        mr = this.gameObject.GetComponent<MeshRenderer>();
        previousMaterials = mr.materials;

        
        mr.material = enlighten;
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
}
