using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractions : MonoBehaviour
{
    public GameObject inHand = null;
    public GameObject player;
    public Transform holdingPoint;

    MoveHandPoints handController;
    GetObjectsNearHand pointer;
    Inventory inventoryScript;

    //cooldowns
    int CooldownClick = 0;
    int CooldownPickUp = 0;
    int CooldownThrow = 0;
    int CooldownSpell = 0;


    private void Start()
    {
        handController = this.GetComponent<MoveHandPoints>();
        pointer = this.GetComponent<GetObjectsNearHand>();
        inventoryScript = this.transform.parent.transform.parent.GetComponent<Inventory>();
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
        if (handController.gesture == "Victory" && inHand == null && CooldownSpell == 0)
        {
            CastSpell();
        }
        if (handController.gesture == "Thumb_Down" && inHand != null)
        {
            AddItemToInventory();
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
        if (CooldownSpell > 0)
        {
            CooldownSpell--;
        }
    }

    void ClickObject()
    {
        if (pointer.currentlyPointing != null)
        {
            CooldownClick = 100;
            if (LayerMask.LayerToName(pointer.currentlyPointing.layer) == "Switch")
            {
                pointer.currentlyPointing.SendMessage("OnClick");
            }
        }
    }

    void PickUpObject()
    {
        if (pointer.currentlyPointing != null)
        {
            CooldownPickUp = 0;
            if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("Item"))
            {
                inHand = pointer.currentlyPointing;
                inHand.transform.SetParent(holdingPoint);
                inHand.transform.localPosition = new Vector3(0, 0, 10);
            }
            if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("UI")) //picking item from inventory
            {
                inHand = inventoryScript.inventory
                    .Find(obj => obj.CompareTag(pointer.currentlyPointing.transform.parent.GetComponent<IconParameters>().iconItem.tag));
                inventoryScript.inventory.Remove(inHand);
                inHand.transform.SetParent(holdingPoint);
                inHand.SetActive(true);
                inHand.transform.localPosition = new Vector3(0, 0, 10);
                inventoryScript.CloseInventory();
            }
        }
    }

    void ThrowObject()
    {
        CooldownThrow = 5;

        if (GetComponent<SpellCasting>().currentSpell == "Light")
        {
            inHand.AddComponent<ThrowSpell>().Initialize(player);
            inHand = null;
            GetComponent<SpellCasting>().currentSpell = "None";
        }
        else
        {
            inHand.AddComponent<ThrowObject>().Initialize(player);
            inHand = null;
        }
    }

    void CastSpell()
    {
        CooldownSpell = 300;

        GetComponent<SpellCasting>().LightSpell();
    }

    void AddItemToInventory()
    {
        if (inHand.layer == LayerMask.NameToLayer("Item"))
        {
            inventoryScript.AddItemFromHand();
        }
    }
}
