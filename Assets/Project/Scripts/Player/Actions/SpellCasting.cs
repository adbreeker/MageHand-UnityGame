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

    private bool isTranscribing = false;

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
            castingFinishedSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);

            currentSpell = "Light";
            PlayerParams.Controllers.handInteractions.inHand = Instantiate(lightPrefab, hand);
            mana -= scroll.manaCost;
        }
        else
        {
            castingFailSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
            castingFailSound.Play();
            Destroy(castingFailSound, castingFailSound.clip.length);
        }
    }

    public IEnumerator PickUpSpell() //casting pick up spell
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Pick Up");
        if(scroll != null)
        {
            castingFinishedSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_SpellPickUpActivation);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);

            Vector3 castPos = PlayerParams.Objects.player.transform.position;
            castPos.y = PlayerParams.Objects.player.transform.position.y - 1.0f;
            Collider[] nearItems = Physics.OverlapSphere(castPos, 3.0f, LayerMask.GetMask("Item"));
            if(nearItems.Length > 0)
            {
                GameObject nearestItem = nearItems[0].gameObject;
                foreach(Collider item in nearItems) 
                {
                    if(Vector3.Distance(item.transform.position, hand.transform.position) < Vector3.Distance(nearestItem.transform.position, hand.transform.position))
                    {
                        nearestItem = item.gameObject;
                    }
                }
                PlayerParams.Controllers.handInteractions.AddToHand(nearestItem, false);
                while (nearestItem.transform.position != PlayerParams.Controllers.handInteractions.holdingPoint.position)
                {
                    nearestItem.transform.position = Vector3.MoveTowards(nearestItem.transform.position, PlayerParams.Controllers.handInteractions.holdingPoint.position, 7.0f * Time.fixedDeltaTime);
                    yield return new WaitForFixedUpdate();
                }
                PlayerParams.Controllers.handInteractions.AddToHand(nearestItem.gameObject, true);
            }
            mana -= scroll.manaCost;
        }
        else
        {
            castingFailSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
            castingFailSound.Play();
            Destroy(castingFailSound, castingFailSound.clip.length);
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

    public void BreakInSpell() //casting break in spell - occurs in tutorial only
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Break In");
        if (scroll != null)
        {
            castingFinishedSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);

            Vector3 castPos = PlayerParams.Objects.player.transform.position;
            Collider[] nearObjects = Physics.OverlapSphere(castPos, 2.0f, LayerMask.GetMask("Default"));
            foreach(Collider potentialLock in nearObjects) 
            {
                if(potentialLock.tag == "Lock")
                {
                    potentialLock.GetComponent<OpenLockedDoorsPassage>().OpenDoors();
                }
            }
            mana -= scroll.manaCost;
        }
        else
        {
            //Later there will be an easter egg
            castingFailSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
            castingFailSound.Play();
            Destroy(castingFailSound, castingFailSound.clip.length);
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
            if (!microphoneRecord.IsRecording && !isTranscribing)
            {
                FindObjectOfType<HUD>().SpawnPopUp("", "Cast a Spell.", timeToFadeOutPopUp, timeOfFadingOutPopUp);

                castingSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_CastingSpell);
                castingSound.Play();

                //Debug.Log("started recording ------------------------ started recording");
                microphoneRecord.StartRecord(GameSettings.microphoneName);
            }
        }
    }

    private async void Transcribe(float[] data, int frequency, int channels, float length) //transcribing speach to text
    {
        isTranscribing = true;

        var res = await whisper.GetTextAsync(data, frequency, channels);

        if (res == null)
            return;

        var spellWhispered = res.Result;
        Debug.Log(NormalizeTranscribedText(spellWhispered));

        FindObjectOfType<HUD>().SpawnPopUp("", "Casting word:<br>" + NormalizeTranscribedTextToDisplay(spellWhispered), timeToFadeOutPopUp, timeOfFadingOutPopUp, false);

        Destroy(castingSound);
        isTranscribing = false;

        CastSpellFromName(spellWhispered);
    }

    public void CastSpellFromName(string name)
    {
        if (NormalizeTranscribedText(name) == "light")
        {
            LightSpell();
        }
        else if (NormalizeTranscribedText(name) == "pickup")
        {
            StartCoroutine(PickUpSpell());
        }
        else if (NormalizeTranscribedText(name) == "fire")
        {
            FireSpell();
        }
        else if (NormalizeTranscribedText(name) == "mark")
        {
            MarkSpell();
        }
        else if (NormalizeTranscribedText(name) == "return")
        {
            ReturnSpell();
        }
        else if (NormalizeTranscribedText(name) == "breakin")
        {
            BreakInSpell();
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


    static string NormalizeTranscribedTextToDisplay(string input)
    {
        // Remove punctuation and convert to lowercase
        string cleanedString = new string(input
            .Where(c => !char.IsPunctuation(c))
            .ToArray())
            .ToLower();

        return cleanedString;
    }
}
