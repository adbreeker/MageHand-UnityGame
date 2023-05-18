using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractions : MonoBehaviour
{
    public GameObject inHand = null;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            if (inHand != null)
            {
                ThrowObject();
            }
            else
            {
                PickUpObject();
            }
        }
    }

    void PickUpObject()
    {

    }

    void ThrowObject()
    {
        if(this.gameObject.GetComponent<SpellCasting>().currentSpell == "Light")
        {
            inHand.AddComponent<ThrowSpell>().Initialize(this.gameObject);
            inHand = null;
            this.gameObject.GetComponent<SpellCasting>().currentSpell = "None";
        }
        else
        {
            inHand.AddComponent<ThrowObject>().Initialize(this.gameObject);
            inHand = null;
        }
    }

}
