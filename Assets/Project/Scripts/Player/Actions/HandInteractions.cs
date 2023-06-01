using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractions : MonoBehaviour
{
    public GameObject inHand = null;
    public GameObject player;

    SimpleTestHand handController;

    private void Start()
    {
        handController = this.GetComponent<SimpleTestHand>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
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
        if(handController.currentlyPointing.layer == LayerMask.NameToLayer("Item"))
        {
            inHand = handController.currentlyPointing;
            inHand.transform.SetParent(this.gameObject.transform);
            inHand.transform.localPosition = new Vector3(0, 0, 2);
        }
    }

    void ThrowObject()
    {
        if(this.gameObject.GetComponentInParent<SpellCasting>().currentSpell == "Light")
        {
            inHand.AddComponent<ThrowSpell>().Initialize(player);
            inHand = null;
            player.GetComponent<SpellCasting>().currentSpell = "None";
        }
        else
        {
            inHand.AddComponent<ThrowObject>().Initialize(player);
            inHand = null;
        }
    }

}
