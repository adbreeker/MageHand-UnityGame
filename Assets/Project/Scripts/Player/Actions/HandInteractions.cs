using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractions : MonoBehaviour
{
    [Header("In Hand")]
    public GameObject inHand = null;

    [Header("Use speach to text")]
    public bool useSpeach = true;

    [Header("Needed objects")]
    public GameObject player;
    public Transform holdingPoint;

    //hand movement and object pointing
    MoveHandPoints handController;
    GetObjectsNearHand pointer;

    //spells and inventory
    SpellCasting spellCastingController;
    Inventory inventoryScript;

    //cooldowns
    bool CooldownClick = false;
    bool CooldownPickUp = false;
    bool CooldownThrow = false;
    bool CooldownCast = false;
    bool CooldownPutDown = false;


    private void Awake()
    {
        handController = GetComponent<MoveHandPoints>();
        pointer = GetComponent<GetObjectsNearHand>();

        spellCastingController = GetComponent<SpellCasting>();
        inventoryScript = transform.parent.transform.parent.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseCooldowns();

        if(handController.gesture == "Pointing_Up" && !CooldownClick)
        {
            ClickObject();
        }

        if(handController.gesture == "Closed_Fist" && inHand == null && !CooldownPickUp)
        {
            PickUpObject();
        }

        if (handController.gesture == "Thumb_Up" && inHand != null && !CooldownThrow)
        {
            ThrowObject();
        }

        if (handController.gesture == "Victory" && inHand == null && spellCastingController.mana == 100 && !CooldownCast)
        {
            CastSpell();
        }

        if (handController.gesture == "Thumb_Down" && inHand != null && !CooldownPutDown)
        {
            PutDownObject();
        }
    }

    void DecreaseCooldowns()
    {
        if(CooldownClick && handController.gesture != "Pointing_Up")
        {
            CooldownClick = false;
        }

        if (CooldownPickUp && handController.gesture != "Closed_Fist")
        {
            CooldownPickUp = false;
        }

        if (CooldownThrow && handController.gesture != "Thumb_Up")
        {
            CooldownThrow = false;
        }

        if (CooldownCast && handController.gesture != "Victory")
        {
            CooldownCast = false;
        }

        if (CooldownPutDown && handController.gesture != "Thumb_Down")
        {
            CooldownPutDown = false;
        }
    }

    void ClickObject()
    {
        if (pointer.currentlyPointing != null)
        {
            CooldownClick = true;
            if (LayerMask.LayerToName(pointer.currentlyPointing.layer) == "Switch")
            {
                pointer.currentlyPointing.SendMessage("OnClick");
            }
            if (LayerMask.LayerToName(pointer.currentlyPointing.layer) == "Chest")
            {
                pointer.currentlyPointing.GetComponent<ChestBehavior>().InteractChest();
            }
        }
    }

    void PickUpObject()
    {
        if (pointer.currentlyPointing != null)
        {
            CooldownPickUp = true;
            if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("Item"))
            {
                inHand = pointer.currentlyPointing;
                inHand.transform.SetParent(holdingPoint);
                inHand.transform.localPosition = new Vector3(0, 0, 10);
                inHand.SendMessage("OnPickUp");
            }
            if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("UI")) //picking item from inventory
            {
                inHand = pointer.currentlyPointing.transform.parent.GetComponent<IconParameters>().originaObject;
                inventoryScript.inventory.Remove(inHand);
                inHand.transform.SetParent(holdingPoint);
                inHand.SetActive(true);
                inHand.transform.localPosition = new Vector3(0, 0, 10);
                inHand.SendMessage("OnPickUp");
                inventoryScript.CloseInventory();
            }
        }
    }

    void ThrowObject()
    {
        CooldownThrow = true;
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
        CooldownCast = true;
        if(useSpeach)
        {
            spellCastingController.RecordSpellCasting();
        }
        else
        {
            spellCastingController.LightSpell();
        }
    }

    void PutDownObject()
    {
        CooldownPutDown = true;
        if (GetComponent<SpellCasting>().currentSpell == "Light")
        {
            MakeFloatingLight();
        }
        else
        {
            AddItemToInventory();
        }
    }

    void AddItemToInventory()
    {
        if (inHand.layer == LayerMask.NameToLayer("Item"))
        {
            inventoryScript.AddItem(inHand);
        }
    }


    // custom interactions while "inserting" spells to inventory

    void MakeFloatingLight() // while trying to insert light to inventory
    {
        inHand.AddComponent<FloatingLight>();
        if(spellCastingController.floatingLight != null)
        {
            Destroy(spellCastingController.floatingLight);
        }
        spellCastingController.floatingLight = inHand;
        inHand = null;
        spellCastingController.currentSpell = "None";
    }
}
