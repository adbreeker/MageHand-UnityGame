using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whisper.Utils;
using Whisper;

public class HandInteractions : MonoBehaviour
{
    [Header("In Hand")]
    public GameObject inHand = null;

    [Header("Speach to text")]
    public string spellWhispered = "";

    [Header("Needed objects")]
    public GameObject player;
    public Transform holdingPoint;

    //hand movement and object pointing
    MoveHandPoints handController;
    GetObjectsNearHand pointer;

    //spells and inventory
    SpellCasting spellCastingController;
    Inventory inventoryScript;

    //speach to text
    MicrophoneRecord microphoneRecord;
    WhisperManager whisper;

    //cooldowns
    int CooldownClick = 0;


    private void Start()
    {
        handController = this.GetComponent<MoveHandPoints>();
        pointer = this.GetComponent<GetObjectsNearHand>();

        spellCastingController = this.GetComponent<SpellCasting>();
        inventoryScript = this.transform.parent.transform.parent.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseCooldowns();
        CastSpell();

        if(handController.gesture == "Pointing_Up" && CooldownClick == 0)
        {
            ClickObject();
        }
        if(handController.gesture == "Closed_Fist" && inHand == null)
        {
            PickUpObject();
        }
        if (handController.gesture == "Thumb_Up" && inHand != null)
        {
            ThrowObject();
        }
        if (handController.gesture == "Victory" && inHand == null && spellCastingController.mana == 100)
        {
            RecordSpellCasting();
        }
        if (handController.gesture == "Thumb_Down" && inHand != null)
        {
            if(GetComponent<SpellCasting>().currentSpell == "Light")
            {
                MakeFloatingLight();
            }
            else
            {
                AddItemToInventory();
            }
        }
    }

    void DecreaseCooldowns()
    {
        if(CooldownClick > 0)
        {
            CooldownClick--;
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
        if(spellWhispered != "")
        {
            spellCastingController.LightSpell();
            spellWhispered = "";
        }
    }

    void AddItemToInventory()
    {
        if (inHand.layer == LayerMask.NameToLayer("Item"))
        {
            inventoryScript.AddItemFromHand();
        }
    }

    void MakeFloatingLight()
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

    //whisper ---------------------------------------------------------------------------------- whisper
    private void Awake()
    {
        microphoneRecord = FindObjectOfType<MicrophoneRecord>();
        whisper = FindObjectOfType<WhisperManager>();

        whisper.InitModel();

        microphoneRecord.OnRecordStop += Transcribe;
    }

    void RecordSpellCasting()
    {
        if (!microphoneRecord.IsRecording)
        {
            Debug.Log("started recording !!!!!!!!!!!!!!!!!!!!!");
            microphoneRecord.StartRecord();
        }
    }

    private async void Transcribe(float[] data, int frequency, int channels, float length)
    {
        var res = await whisper.GetTextAsync(data, frequency, channels);

        if (res == null)
            return;

        var text = res.Result;
        spellWhispered = text;

        //spellWhispered = res.Result;
    }
}
