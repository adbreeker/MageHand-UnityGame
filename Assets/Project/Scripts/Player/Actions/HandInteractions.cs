using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;

public class HandInteractions : MonoBehaviour
{
    [Header("In Hand")]
    public GameObject inHand = null;

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

    private AudioSource pickUpItemSound;
    private AudioSource putToInventorySound;
    private AudioSource drinkSound;

    LayerMask inHandPreviousLayer;

    [Header("Pop up options")]
    public float timeToFadeOutPopUp = 1;
    public float timeOfFadingOutPopUp = 0.007f;


    private void Awake() //get necessary components
    {
        gestureHandler = GetComponent<MoveHandPoints>();
        pointer = GetComponent<GetObjectsNearHand>();

        pickUpItemSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_PickUpItem);
        putToInventorySound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_PutToInventory);
        drinkSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Drink);
    }

    void Update()
    {
        DecreaseCooldowns(); //decrease cooldowns for all actions

        //listen to player gestures
        if (gestureHandler.gesture == "Pointing_Up" && !CooldownClick)
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
            if (LayerMask.LayerToName(pointer.currentlyPointing.layer) == "Interaction" || 
                (LayerMask.LayerToName(pointer.currentlyPointing.layer) == "UI" && PlayerParams.Controllers.spellsMenu.menuOpened))
            {
                pointer.currentlyPointing.SendMessage("OnClick");
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
                AddToHand(pointer.currentlyPointing, true, false);
            }
            if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("UI") 
                && PlayerParams.Controllers.inventory.inventoryOpened) //picking item from inventory
            {
                if (pointer.currentlyPointing.GetComponent<ReadableBehavior>() == null && pointer.currentlyPointing.GetComponent<PopUpActivateOnPickUp>() == null) pickUpItemSound.Play();

                //getting item from inventory
                GameObject itemFromInventory = pointer.currentlyPointing.transform.parent.GetComponent<IconParameters>().originalObject;
                PlayerParams.Controllers.inventory.inventory.Remove(itemFromInventory);
                itemFromInventory.SetActive(true);
                AddToHand(itemFromInventory, true, false);

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
            //set proper layer
            ChangeLayer(inHand, inHandPreviousLayer);

            inHand.AddComponent<ThrowSpell>().Initialize(player);

            inHand = null;
            GetComponent<SpellCasting>().currentSpell = "None";
        }
        else //else throw item
        {
            //set proper layer
            ChangeLayer(inHand, inHandPreviousLayer);

            inHand.AddComponent<ThrowObject>().Initialize(player);

            inHand = null;
        }
    }

    void CastSpell() //cast spell with SpellCasting class
    {
        CooldownCast = true;
        if (PlayerParams.Controllers.spellbook.spells.Count > 0)
        {
            if (GameSettings.useSpeech && !PlayerParams.Variables.uiActive) //if using speach then microphone starting to record
            {
                PlayerParams.Controllers.playerManager.StartCoroutine(PlayerParams.Controllers.spellCasting.CastSpell());
            }
            else if (!PlayerParams.Variables.uiActive) //open spells menu if using speech is off
            {
                PlayerParams.Controllers.spellsMenu.OpenMenu();
            }
        }
    }

    public void PutDownObject() //put object down to inventory or if in hand is spell then some special interaction
    {
        CooldownPutDown = true;
        CooldownPickUp = true;

        //set proper layer
        ChangeLayer(inHand, inHandPreviousLayer);

        if (PlayerParams.Controllers.spellCasting.currentSpell == "Light") //if light spell in hand, making it floating light
        {
            MakeFloatingLight();
        }
        else
        {
            if (inHand.layer == LayerMask.NameToLayer("Item")) //if item in hand then just putting it down to inventory
            {
                putToInventorySound.Play();
                PlayerParams.Controllers.inventory.AddItem(inHand);
            }
        }
    }

    void DrinkObject() //drink object from hand, most likely potion
    {
        if(inHand.tag == "Potion")
        {
            drinkSound.Play();
            inHand.GetComponent<PotionEffect>().Drink();
            inHand = null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------
    //additional interactions methods

    public void AddToHand(GameObject toHand, bool withPositionChange, bool isSpell)
    {
        if (toHand.GetComponent<ReadableBehavior>() == null && toHand.GetComponent<PopUpActivateOnPickUp>() == null && withPositionChange && !isSpell)
        {
            pickUpItemSound.Play();
        }

        inHand = toHand;

        //making item a child of hand so it will move when hand is moving
        inHand.transform.SetParent(holdingPoint);
        if (withPositionChange) 
        {
            if(isSpell) { inHand.transform.localPosition = new Vector3(0, 0, 5); }
            else { inHand.transform.localPosition = new Vector3(0, 0, 10); }
        }

        //setting UI layer
        inHandPreviousLayer = inHand.layer;
        ChangeLayer(inHand, LayerMask.NameToLayer("UI"));

        //invoking OnPickUp method of picked item
        if (!isSpell) { inHand.GetComponent<ItemBehavior>().OnPickUp(); }
    }

    void ChangeLayer(GameObject obj ,LayerMask layer)
    {
        //making inHand UI layer to prevent it from disappearing in the walls
        if (obj != null)
        {
            obj.layer = layer;
            //Debug.Log("Changing layer of:" + obj.name);
            foreach (Transform child in obj.transform)
            {
                ChangeLayer(child.gameObject, layer);
            }
        }
    }


    // custom interactions while "inserting" spells to inventory -------------------------------------------------------------------------------------------

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
