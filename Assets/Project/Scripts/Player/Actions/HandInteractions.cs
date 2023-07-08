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
    MoveHandPoints gestureHandler;
    GetObjectsNearHand pointer;

    //cooldowns
    bool CooldownClick = false;
    bool CooldownPickUp = false;
    bool CooldownThrow = false;
    bool CooldownCast = false;
    bool CooldownPutDown = false;
    bool CooldownDrink = false;


    private void Awake() //get necessary components
    {
        gestureHandler = GetComponent<MoveHandPoints>();
        pointer = GetComponent<GetObjectsNearHand>();
    }

    void Update()
    {
        DecreaseCooldowns(); //decrease cooldowns for all actions

        //listen to player gestures
        if(gestureHandler.gesture == "Pointing_Up" && !CooldownClick)
        {
            ClickObject();
        }

        if(gestureHandler.gesture == "Closed_Fist" && inHand == null && !CooldownPickUp)
        {
            PickUpObject();
        }

        if (gestureHandler.gesture == "Thumb_Up" && inHand != null && !CooldownThrow)
        {
            ThrowObject();
        }

        if (gestureHandler.gesture == "Victory" && inHand == null && PlayerParams.Controllers.spellCasting.mana == 100 && !CooldownCast)
        {
            CastSpell();
        }

        if (gestureHandler.gesture == "Thumb_Down" && inHand != null && !CooldownPutDown)
        {
            PutDownObject();
        }

        if (gestureHandler.gesture == "ILoveYou" && inHand != null && !CooldownDrink)
        {
            DrinkObject();
        }
    }

    void DecreaseCooldowns() //check if gesture was changed, if yes - reset cooldown of that gesture
    {
        if(CooldownClick && gestureHandler.gesture != "Pointing_Up")
        {
            CooldownClick = false;
        }

        if (CooldownPickUp && gestureHandler.gesture != "Closed_Fist")
        {
            CooldownPickUp = false;
        }

        if (CooldownThrow && gestureHandler.gesture != "Thumb_Up")
        {
            CooldownThrow = false;
        }

        if (CooldownCast && gestureHandler.gesture != "Victory")
        {
            CooldownCast = false;
        }

        if (CooldownPutDown && gestureHandler.gesture != "Thumb_Down")
        {
            CooldownPutDown = false;
        }
        if (CooldownDrink && gestureHandler.gesture != "ILoveYou")
        {
            CooldownDrink = false;
        }
    }

    void ClickObject() //interact with objects you would normally click, like switches or chests
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

    void PickUpObject() //pick up pointed item from scene or inventory
    {
        if (pointer.currentlyPointing != null)
        {
            CooldownPickUp = true;
            if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("Item")) //picking item from scene
            {
                inHand = pointer.currentlyPointing;

                //making item a child of hand so it will move when hand is moving
                inHand.transform.SetParent(holdingPoint);
                inHand.transform.localPosition = new Vector3(0, 0, 10);

                //invoking OnPickUp method of picked item
                inHand.SendMessage("OnPickUp");
            }
            if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("UI")) //picking item from inventory
            {
                //getting item from inventory
                inHand = pointer.currentlyPointing.transform.parent.GetComponent<IconParameters>().originaObject;
                PlayerParams.Controllers.inventory.inventory.Remove(inHand);

                //activing item and making it a child of hand so it will move when hand is moving
                inHand.transform.SetParent(holdingPoint);
                inHand.SetActive(true);
                inHand.transform.localPosition = new Vector3(0, 0, 10);

                //invoking OnPickUp method of picked item
                inHand.SendMessage("OnPickUp");

                //closing inventory
                PlayerParams.Controllers.inventory.CloseInventory();
            }
        }
    }

    void ThrowObject() //throw item or spell
    {
        CooldownThrow = true;
        string cs = GetComponent<SpellCasting>().currentSpell;

        if (cs == "Light" || cs == "Fire") //if spell then throw spell
        {
            inHand.AddComponent<ThrowSpell>().Initialize(player);
            inHand = null;
            GetComponent<SpellCasting>().currentSpell = "None";
        }
        else //else throw item
        {
            inHand.AddComponent<ThrowObject>().Initialize(player);
            inHand = null;
        }
    }

    void CastSpell() //cast spell with SpellCasting class
    {
        CooldownCast = true;
        if(useSpeach) //if using speach then microphone starting to record
        {
            PlayerParams.Controllers.spellCasting.RecordSpellCasting();
        }
        else //else just instantly cast light spell
        {
            PlayerParams.Controllers.spellCasting.LightSpell();
        }
    }

    void PutDownObject() //put object down to inventory or if in hand is spell then some special interaction
    {
        CooldownPutDown = true;
        if (PlayerParams.Controllers.spellCasting.currentSpell == "Light") //if light spell in hand, making it floating light
        {
            MakeFloatingLight();
        }
        else
        {
            if (inHand.layer == LayerMask.NameToLayer("Item")) //if item in hand then just putting it down to inventory
            {
                PlayerParams.Controllers.inventory.AddItem(inHand);
            }
        }
    }

    void DrinkObject() //drink object from hand, most likely potion
    {
        if(inHand.tag == "Potion")
        {
            inHand.SendMessage("Drink");
            inHand = null;
        }
    }


    // custom interactions while "inserting" spells to inventory

    void MakeFloatingLight() // while trying to insert light to inventory, makes it float around player for some time
    {
        inHand.AddComponent<FloatingLight>();

        if(PlayerParams.Controllers.spellCasting.floatingLight != null) //if floatin light actually exists then replacing it
        {
            Destroy(PlayerParams.Controllers.spellCasting.floatingLight);
        }
        PlayerParams.Controllers.spellCasting.floatingLight = inHand;
        inHand = null;
        PlayerParams.Controllers.spellCasting.currentSpell = "None";
    }
}
