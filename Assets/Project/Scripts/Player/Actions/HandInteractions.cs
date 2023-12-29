using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using System.IO.Pipes;
using System;

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

    private string word;
    
    private int recordingTime = 2; // Set the recording time in seconds
   

    private AudioSource pickUpItemSound;
    private AudioSource putToInventorySound;
    private AudioSource drinkSound;
    public AudioSource castingSound;
    private AudioClip recordedClip;

    public float timeToFadeOutPopUp = 1;
    public float timeOfFadingOutPopUp = 0.007f;
    public bool canCastNewSpell = true;


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
        if(Input.GetKeyDown(KeyCode.U))
        {
            PlayerParams.Variables.uiActive = !PlayerParams.Variables.uiActive;
            Debug.Log(PlayerParams.Variables.uiActive);
        }
        
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
            if (LayerMask.LayerToName(pointer.currentlyPointing.layer) == "Switch" || 
                (LayerMask.LayerToName(pointer.currentlyPointing.layer) == "UI" && PlayerParams.Controllers.spellsMenu.menuOpened))
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
                AddToHand(pointer.currentlyPointing, true);
            }
            if (pointer.currentlyPointing.layer == LayerMask.NameToLayer("UI") 
                && PlayerParams.Controllers.inventory.inventoryOpened) //picking item from inventory
            {
                if (pointer.currentlyPointing.GetComponent<ReadableBehavior>() == null && pointer.currentlyPointing.GetComponent<PopUpActivateOnPickUp>() == null) pickUpItemSound.Play();
                //getting item from inventory
                inHand = pointer.currentlyPointing.transform.parent.GetComponent<IconParameters>().originalObject;
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
        if (canCastNewSpell && PlayerParams.Controllers.spellbook.spells.Count > 0)
        {
            if (GameSettings.useSpeech && !PlayerParams.Variables.uiActive) //if using speach then microphone starting to record
            {
                recordedClip = Microphone.Start(GameSettings.microphoneName, false, recordingTime, 16000);

                // Wait for the specified recording time
                StartCoroutine(WaitForRecording());

            }
            else if (!PlayerParams.Variables.uiActive) //open spells menu if using speech is off
            {
                PlayerParams.Controllers.spellsMenu.OpenMenu();
            }
        }
    }

    IEnumerator WaitForRecording()
    {
        canCastNewSpell = false;

        // PopUp cast spell
        Debug.Log("Whisper listening");
        FindObjectOfType<HUD>().SpawnPopUp("", "Cast a Spell.", timeToFadeOutPopUp, timeOfFadingOutPopUp);
        castingSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpell);
        castingSound.Play();

        // Wait for the specified recording time
        yield return new WaitForSeconds(recordingTime);

        Microphone.End(GameSettings.microphoneName);

        byte[] audioData = ConvertAudioClipToByteArray(recordedClip);

        MemoryMappedFile mmf_audio = MemoryMappedFile.OpenExisting("magehand_whisper_audio");
        MemoryMappedViewStream stream_audio= mmf_audio.CreateViewStream();
        BinaryWriter write_audio= new BinaryWriter(stream_audio);
  
        write_audio.Write(audioData, 0, audioData.Length);

        PlayerParams.Controllers.spellCasting.WriteToMemoryMappedFile("magehand_whisper_run", "ok");

        StartCoroutine(PlayerParams.Controllers.spellCasting.WaitForSpell());
    }

    byte[] ConvertAudioClipToByteArray(AudioClip clip)
    {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        Byte[] bytesData = new Byte[samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        float rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        return bytesData;
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
            inHand.SendMessage("Drink");
            inHand = null;
        }
    }

    public void AddToHand(GameObject toHand, bool withPositionChange)
    {
        if (toHand.GetComponent<ReadableBehavior>() == null && toHand.GetComponent<PopUpActivateOnPickUp>() == null && withPositionChange)
        {
            pickUpItemSound.Play();
        }

        inHand = toHand;

        //making item a child of hand so it will move when hand is moving
        inHand.transform.SetParent(holdingPoint);
        if (withPositionChange) 
        {
            inHand.transform.localPosition = new Vector3(0, 0, 10);
        }

        //invoking OnPickUp method of picked item
        inHand.SendMessage("OnPickUp");
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
