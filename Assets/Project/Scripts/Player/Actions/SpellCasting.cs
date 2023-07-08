using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whisper;
using Whisper.Utils;
using System.Linq;

public class SpellCasting : MonoBehaviour
{
    [Header("Spellbook")]
    public Spellbook spellbook;

    [Header("Mana")]
    public float mana = 100.0f;
    public float manaRegen = 3.0f;

    [Header("Current Spell")]
    public string currentSpell = "None";
    public GameObject floatingLight;

    [Header("Spell Cast Position")]
    public Transform hand;

    [Header("Spells Prefabs")]
    public GameObject lightPrefab;
    public GameObject firePrefab;

    //classes necessary for speach to text
    private MicrophoneRecord microphoneRecord;
    private WhisperManager whisper;

    private void FixedUpdate()
    {
        RegenerateMana(); //regenerating mana every fixed update
    }

    void RegenerateMana() //regenerating mana with use of mana regen parameter
    {
        mana += manaRegen * Time.deltaTime;
        mana = Mathf.Clamp(mana, 0.0f, 100.0f);
    }

    //spells --------------------------------------------------------------------------------------- spells

    public void LightSpell() //casting light spell
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Light");
        if(scroll != null)
        {
            currentSpell = "Light";
            PlayerParams.Controllers.handInteractions.inHand = Instantiate(lightPrefab, hand);
            mana -= scroll.manaCost;
        }
    }

    public void FireSpell() //casting fire spell
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Fire");
        if (scroll != null)
        {
            currentSpell = "Fire";
            PlayerParams.Controllers.handInteractions.inHand = Instantiate(firePrefab, hand);
            mana -= scroll.manaCost;
        }
    }


    //whisper --------------------------------------------------------------------------------------------- whisper

    private void Awake() //initiation on awake
    {
        microphoneRecord = FindObjectOfType<MicrophoneRecord>();
        whisper = FindObjectOfType<WhisperManager>();

        _ = whisper.InitModel();

        microphoneRecord.OnRecordStop += Transcribe;
    }

    public void RecordSpellCasting() //recording player speach
    {
        if(spellbook.spells.Count > 0)
        {
            if (!microphoneRecord.IsRecording)
            {
                Debug.Log("started recording ------------------------ started recording");
                microphoneRecord.StartRecord();
            }
        }
    }

    private async void Transcribe(float[] data, int frequency, int channels, float length) //transcribing speach to text
    {
        var res = await whisper.GetTextAsync(data, frequency, channels);

        if (res == null)
            return;

        var spellWhispered = res.Result;
        Debug.Log(NormalizeTranscribedText(spellWhispered));

        if(NormalizeTranscribedText(spellWhispered) == "light")
        {
            LightSpell();
        }
        if (NormalizeTranscribedText(spellWhispered) == "fire")
        {
            FireSpell();
        }
    }

    private string NormalizeTranscribedText(string text) //normalizing transcribed speach
    {
        string normalized = new string(text
            .Where(c => char.IsLetter(c))
            .Select(c => char.ToLowerInvariant(c))
            .ToArray());

        return normalized;
    }

}
