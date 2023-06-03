using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractions : MonoBehaviour
{
    public GameObject inHand = null;
    public GameObject player;
    public Transform holdingPoint;

    MoveHandPoints handController;
    RaycastFromHand pointer;

    //cooldowns
    int CooldownClick = 0;
    int CooldownPickUp = 0;
    int CooldownThrow = 0;


    private void Start()
    {
        handController = this.GetComponent<MoveHandPoints>();
        pointer = this.GetComponent<RaycastFromHand>();
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseCooldowns();

        if(handController.gesture == "Pointing_Up" && CooldownClick == 0)
        {
            ClickObject();
        }
        if(handController.gesture == "Closed_Fist" && inHand == null && CooldownPickUp == 0)
        {
            PickUpObject();
        }
        if (handController.gesture == "Thumb_Up" && inHand != null && CooldownThrow == 0)
        {
            ThrowObject();
        }
    }

    void DecreaseCooldowns()
    {
        if(CooldownClick > 0)
        {
            CooldownClick--;
        }
        if (CooldownPickUp > 0)
        {
            CooldownPickUp--;
        }
        if (CooldownThrow > 0)
        {
            CooldownThrow--;
        }
    }

    void ClickObject()
    {
        CooldownClick = 100;

        if (LayerMask.LayerToName(pointer.currentlyPointing.layer) == "Switch")
        {
            pointer.currentlyPointing.SendMessage("OnClick");
        }
    }

    void PickUpObject()
    {
        CooldownPickUp = 5;

        if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("Item"))
        {
            inHand = pointer.currentlyPointing;
            inHand.transform.SetParent(holdingPoint);
            inHand.transform.localPosition = new Vector3(0, 0, 2);
        }
    }

    void ThrowObject()
    {
        CooldownThrow = 5;

        if (this.gameObject.GetComponentInParent<SpellCasting>().currentSpell == "Light")
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
