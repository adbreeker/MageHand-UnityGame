using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whisper;
using Whisper.Utils;
using System.Linq;
using UnityEngine.Rendering;

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
    public GameObject magicMark;

    [Header("Spell Cast Position")]
    public Transform hand;

    [Header("Pop up options")]
    public GameObject popUpPrefab;
    public float timeToFadeOutPopUp = 1;
    public float timeOfFadingOutPopUp = 0.007f;

    [Header("Spells Prefabs")]
    public GameObject lightPrefab;
    public GameObject firePrefab;
    public GameObject markPrefab;


    private AudioSource castingSound;
    private AudioSource castingFailSound;
    private AudioSource castingFinishedSound;

    //classes necessary for speach to text
    private MicrophoneRecord microphoneRecord;
    private WhisperManager whisper;

    private void Start()
    {
        PlayerParams.Variables.startingManaRegen = manaRegen;
    }

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

    public void PickUpSpell()
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Pick Up");
        if(scroll != null)
        {
            Vector3 castPos = PlayerParams.Objects.player.transform.position;
            castPos.y = PlayerParams.Objects.player.transform.position.y - 1.0f;
            Collider[] nearItems = Physics.OverlapSphere(castPos, 3.0f, LayerMask.GetMask("Item"));
            if(nearItems.Length > 0)
            {
                PlayerParams.Controllers.handInteractions.AddToHand(nearItems[0].gameObject);
            }
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

    public void MarkSpell() //marking place under player for future teleportation
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Mark And Return");
        if (scroll != null)
        {
            if (PlayerParams.Controllers.playerMovement.isMoving)
            {
                Vector3 place = PlayerParams.Controllers.playerMovement.currentTilePos;
                place.y = 0;
                magicMark = Instantiate(markPrefab, place, Quaternion.identity);
            }
            else
            {
                Vector3 place = PlayerParams.Objects.player.transform.position;
                place.y = 0;
                magicMark = Instantiate(markPrefab, place, Quaternion.identity);
            }
            mana -= scroll.manaCost / 4;
        }
    }

    public void ReturnSpell() //teleporting to marked place, if mark exists
    {
        if (magicMark != null)
        {
            SpellScrollInfo scroll = spellbook.GetSpellInfo("Mark And Return");
            if (scroll != null)
            {
                Vector3 tpDestination = magicMark.transform.position;
                tpDestination.y = 1;
                PlayerParams.Controllers.playerMovement.stopMovement = true;
                PlayerParams.Controllers.playerMovement.TeleportTo(tpDestination);
                magicMark.GetComponent<MarkAndReturnSpellBehavior>().TeleportationPerformed();
                mana -= scroll.manaCost;
            }
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
                GameObject popUp = Instantiate(popUpPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                popUp.GetComponent<PopUp>().ActivatePopUp("", "Cast a Spell.", timeToFadeOutPopUp, timeOfFadingOutPopUp);

                castingSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpell);
                castingSound.Play();

                //Debug.Log("started recording ------------------------ started recording");
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

        GameObject popUp = Instantiate(popUpPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        popUp.GetComponent<PopUp>().ActivatePopUp("", "Detected word:<br>" + NormalizeTranscribedText(spellWhispered), timeToFadeOutPopUp, timeOfFadingOutPopUp, false);

        Destroy(castingSound);

        if (NormalizeTranscribedText(spellWhispered) == "light")
        {
            castingFinishedSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);
            LightSpell();
        }
        else if (NormalizeTranscribedText(spellWhispered) == "pickup")
        {
            castingFinishedSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_SpellPickUpActivation);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);
            PickUpSpell();
        }
        else if(NormalizeTranscribedText(spellWhispered) == "fire")
        {
            castingFinishedSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);
            FireSpell();
        }
        else if(NormalizeTranscribedText(spellWhispered) == "mark")
        {
            castingFinishedSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);
            MarkSpell();
        }
        else if (NormalizeTranscribedText(spellWhispered) == "return")
        {
            castingFinishedSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);
            ReturnSpell();
        }
        else
        {
            castingFailSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
            castingFailSound.Play();
            Destroy(castingFailSound, castingFailSound.clip.length);
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
