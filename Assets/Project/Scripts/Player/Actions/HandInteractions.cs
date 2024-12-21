using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using FMODUnity;

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

    LayerMask inHandPreviousLayer;

    [Header("Pop up options")]
    public float timeToFadeOutPopUp = 1;
    public float timeOfFadingOutPopUp = 0.007f;

    FmodEvents FmodEvents => GameParams.Managers.fmodEvents;

    private void Awake() //get necessary components
    {
        gestureHandler = GetComponent<MoveHandPoints>();
        pointer = GetComponent<GetObjectsNearHand>();
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
                if (pointer.currentlyPointing.GetComponent<ReadableBehavior>() == null 
                    && pointer.currentlyPointing.GetComponent<PopUpActivateOnPickUp>() == null)
                {
                    RuntimeManager.PlayOneShot(FmodEvents.SFX_PickUpItem);
                }

                //getting item from inventory
                GameObject itemFromInventory = pointer.currentlyPointing.transform.parent.GetComponent<IconParameters>().originalObject;
                PlayerParams.Controllers.inventory.inventory.Remove(itemFromInventory);
                itemFromInventory.SetActive(true);

                //closing inventory
                PlayerParams.Controllers.inventory.CloseInventory();

                //adding item to hand after closing inventory (when it is readable, it needs to turn off movement after turning it on by closing inventory)
                AddToHand(itemFromInventory, true, false);
            }
        }
    }

    void ThrowObject() //throw item or spell
    {
        CooldownThrow = true;

        if (PlayerParams.Controllers.spellCasting.currentSpell == "Light" 
            || PlayerParams.Controllers.spellCasting.currentSpell == "Fire" 
            || PlayerParams.Controllers.spellCasting.currentSpell == "Mark") //if spell then throw spell
        {
            //set proper layer
            ChangeLayer(inHand, inHandPreviousLayer);

            if (PlayerParams.Controllers.playerMovement.isMoving)
            {
                Vector3? obstacle = GetThrowingObstacle();
                if (obstacle.HasValue) { inHand.transform.position = obstacle.Value; }
            }
            inHand.AddComponent<ThrowSpell>().Initialize(PlayerParams.Objects.playerCamera.transform.forward);

            inHand = null;
            PlayerParams.Controllers.spellCasting.currentSpell = "None";
        }
        else //else throw item
        {
            //set proper layer
            ChangeLayer(inHand, inHandPreviousLayer);

            if (PlayerParams.Controllers.playerMovement.isMoving)
            {
                Vector3? obstacle = GetThrowingObstacle();
                if (obstacle.HasValue) { inHand.transform.position = obstacle.Value; }
            }
            inHand.AddComponent<ThrowObject>().Initialize(PlayerParams.Objects.playerCamera.transform.forward);

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

        if (inHand.layer == LayerMask.NameToLayer("Item")) //if item in hand then just putting it down to inventory
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_PutToInventory);
            PlayerParams.Controllers.inventory.AddItem(inHand);
        }
        else if (inHand.layer == LayerMask.NameToLayer("Spell")) //if spell in hand use reactivation
        {
            inHand.GetComponent<SpellBehavior>().Reactivation();
        }
        
    }

    void DrinkObject() //drink object from hand, most likely potion
    {
        if(inHand.tag == "Potion")
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_Drink);
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
            RuntimeManager.PlayOneShot(FmodEvents.SFX_PickUpItem);
        }

        inHand = toHand;

        //making item a child of hand so it will move when hand is moving
        inHand.transform.SetParent(holdingPoint);
        if (withPositionChange) 
        {
            if(isSpell) { inHand.transform.localPosition = new Vector3(0, 0, 5); }
            else { inHand.transform.localPosition = new Vector3(0, 0, 10); }
        }

        inHand.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        //setting UI layer
        inHandPreviousLayer = inHand.layer;
        ChangeLayer(inHand, LayerMask.NameToLayer("UI"));

        //invoking OnPickUp method of picked item
        if (!isSpell) { inHand.SendMessage("OnPickUp"); }
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

    Vector3? GetThrowingObstacle()
    {
        Vector3 rayOrigin = holdingPoint.transform.TransformPoint(
            new Vector3(
                holdingPoint.localPosition.x, 
                holdingPoint.localPosition.y, 
                holdingPoint.localPosition.z - 1/holdingPoint.lossyScale.z
            )
        );
        Ray ray = new Ray(rayOrigin, (holdingPoint.position - rayOrigin));
        //Debug.DrawRay(ray.origin, ray.direction * 1.1f, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(ray, 1.1f, LayerMask.GetMask("Default", "Interaction", "Spell"), QueryTriggerInteraction.Ignore);
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Obstacle" || hit.collider.tag == "Wall")
            {
                return hit.point + Vector3.Scale(ray.direction, -0.1f * Vector3.one);
            }
        }

        return null;
    }
}
