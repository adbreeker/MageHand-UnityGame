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


    private MicrophoneRecord microphoneRecord;
    private WhisperManager whisper;

    private void FixedUpdate()
    {
        RegenerateMana();
    }

    void RegenerateMana()
    {
        mana += manaRegen * Time.deltaTime;
        mana = Mathf.Clamp(mana, 0.0f, 100.0f);
    }

    //spells -------------------------------------------------------

    public void LightSpell()
    {
        SpellScrollInfo? scroll = spellbook.GetSpellInfo("Light");
        if(scroll != null)
        {
            currentSpell = "Light";
            GetComponent<HandInteractions>().inHand = Instantiate(lightPrefab, hand);
            mana -= ((SpellScrollInfo)scroll).manaCost;
        }
    }

    public void FireSpell()
    {
        SpellScrollInfo? scroll = spellbook.GetSpellInfo("Fire");
        if (scroll != null)
        {
            currentSpell = "Fire";
            GetComponent<HandInteractions>().inHand = Instantiate(firePrefab, hand);
            mana -= ((SpellScrollInfo)scroll).manaCost;
        }
    }


    //whisper --------------------------------------------------------

    private void Awake()
    {
        microphoneRecord = FindObjectOfType<MicrophoneRecord>();
        whisper = FindObjectOfType<WhisperManager>();

        whisper.InitModel();

        microphoneRecord.OnRecordStop += Transcribe;
    }

    public void RecordSpellCasting()
    {
        if(spellbook.spells.Count > 0)
        {
            if (!microphoneRecord.IsRecording)
            {
                Debug.Log("started recording !!!!!!!!!!!!!!!!!!!!!");
                microphoneRecord.StartRecord();
            }
        }
    }

    private async void Transcribe(float[] data, int frequency, int channels, float length)
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

    private string NormalizeTranscribedText(string text)
    {
        string normalized = new string(text
            .Where(c => char.IsLetter(c))
            .Select(c => char.ToLowerInvariant(c))
            .ToArray());

        return normalized;
    }

}
