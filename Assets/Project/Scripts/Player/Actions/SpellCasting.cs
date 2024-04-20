using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.IO.Pipes;
using System;


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

    [Header("Easter Egg")]
    public GameObject openEasterEggPrefab;

    //private AudioSource castingSound;
    private AudioSource castingFailSound;
    private AudioSource castingFinishedSound;

    //private bool isTranscribing = false;

    private bool _isAbleToCast = true;

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
            castingFinishedSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);

            currentSpell = "Light";
            PlayerParams.Controllers.handInteractions.AddToHand(Instantiate(lightPrefab), true, true);
            mana -= scroll.manaCost;
        }
        else
        {
            castingFailSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
            castingFailSound.Play();
            Destroy(castingFailSound, castingFailSound.clip.length);
        }
    }

    public IEnumerator PickUpSpell() //casting pick up spell
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Collect");
        if(scroll != null)
        {
            castingFinishedSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_SpellPickUpActivation);
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
                PlayerParams.Controllers.handInteractions.AddToHand(nearestItem, false, false);
                while (nearestItem.transform.position != PlayerParams.Controllers.handInteractions.holdingPoint.position)
                {
                    nearestItem.transform.position = Vector3.MoveTowards(nearestItem.transform.position, PlayerParams.Controllers.handInteractions.holdingPoint.position, 7.0f * Time.fixedDeltaTime);
                    yield return new WaitForFixedUpdate();
                }
                nearestItem.layer = LayerMask.NameToLayer("Item");
                PlayerParams.Controllers.handInteractions.AddToHand(nearestItem.gameObject, true, false);
            }
            mana -= scroll.manaCost;
        }
        else
        {
            castingFailSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
            castingFailSound.Play();
            Destroy(castingFailSound, castingFailSound.clip.length);
        }
    }

    public void FireSpell() //casting fire spell
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Fire");
        if (scroll != null)
        {
            castingFinishedSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);

            currentSpell = "Fire";
            PlayerParams.Controllers.handInteractions.AddToHand(Instantiate(firePrefab), true, true);
            mana -= scroll.manaCost;
        }
        else
        {
            castingFailSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
            castingFailSound.Play();
            Destroy(castingFailSound, castingFailSound.clip.length);
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
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Open");
        if (scroll != null)
        {
            castingFinishedSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFinished);
            castingFinishedSound.Play();
            Destroy(castingFinishedSound, castingFinishedSound.clip.length);

            Vector3 castPos = PlayerParams.Objects.player.transform.position;
            Collider[] nearObjects = Physics.OverlapSphere(castPos, 2.0f, LayerMask.GetMask("Default"));
            foreach(Collider potentialLock in nearObjects) 
            {
                if(potentialLock.tag == "Lock")
                {
                    potentialLock.GetComponent<LockBehavior>().OpenLock();
                }
            }
            mana -= scroll.manaCost;
        }
        else
        {
            castingFailSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
            castingFailSound.Play();
            Destroy(castingFailSound, castingFailSound.clip.length);

            Instantiate(openEasterEggPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        }
    }

    //whisper --------------------------------------------------------------------------------------------- whisper
    public IEnumerator CastSpell()
    {
        if(!_isAbleToCast)
        {
            Debug.Log("Whisper is currently working - preventing new cast");
            yield break;
        }

        _isAbleToCast = false;

        AudioClip recordedClip = Microphone.Start(GameSettings.microphoneName, false, 2, 16000);

        // PopUp cast spell
        Debug.Log("Whisper listening");
        FindObjectOfType<HUD>().SpawnPopUp("Cast a Spell.", timeToFadeOutPopUp, timeOfFadingOutPopUp);
        AudioSource castingSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpell);
        castingSound.Play();

        // Wait for the specified recording time
        yield return new WaitForSecondsRealtime(2.0f);

        Microphone.End(GameSettings.microphoneName);

        byte[] audioData = ConvertAudioClipToByteArray(recordedClip);

        MemoryMappedFile mmf_audio = MemoryMappedFile.OpenExisting("magehand_whisper_audio");
        MemoryMappedViewStream stream_audio = mmf_audio.CreateViewStream();
        BinaryWriter write_audio = new BinaryWriter(stream_audio);

        write_audio.Write(audioData, 0, audioData.Length);

        PlayerParams.Controllers.spellCasting.WriteToMemoryMappedFile("magehand_whisper_run", "ok");

        WriteToMemoryMappedFile("magehand_whisper_text", "None");

        string okString = "ok";

        while (okString == "ok")
        {
            byte[] frameGesture;
            ReadFromMemoryMappedFile("magehand_whisper_run", 2, out frameGesture);
            okString = System.Text.Encoding.UTF8.GetString(frameGesture, 0, 2);
            yield return new WaitForFixedUpdate();
        }

        byte[] frameWord;
        ReadFromMemoryMappedFile("magehand_whisper_text", 10, out frameWord);

        string word = System.Text.Encoding.UTF8.GetString(frameWord).Split(";")[0];
        Debug.Log("Whisper transcribed word: " + word);

        if (word.Length >= 4 && word.Substring(0, 4) == "None") FindObjectOfType<HUD>().SpawnPopUp("Casting word:<br>(silence)", timeToFadeOutPopUp, timeOfFadingOutPopUp, false);
        else FindObjectOfType<HUD>().SpawnPopUp("Casting word:<br>" + word, timeToFadeOutPopUp, timeOfFadingOutPopUp, false);

        Destroy(castingSound);

        WriteToMemoryMappedFile("magehand_whisper_run", "no");

        CastSpellFromName(word);
        _isAbleToCast = true;
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


    //old
    /*
    private void Awake() //initiation on awake
    {
        //microphoneRecord = FindObjectOfType<MicrophoneRecord>();
        //whisper = FindObjectOfType<WhisperManager>();

        //_ = whisper.InitModel();

        //microphoneRecord.OnRecordStop += Transcribe;
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
    */

    public void CastSpellFromName(string name)
    {
        if (NormalizeTranscribedText(name) == "light")
        {
            LightSpell();
        }
        else if (NormalizeTranscribedText(name) == "collect")
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
        else if (NormalizeTranscribedText(name) == "open")
        {
            BreakInSpell();
        }
        else
        {
            castingFailSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpellFailed);
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

    public void WriteToMemoryMappedFile(string mapName, string data)
    {
        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mapName))
        using (MemoryMappedViewStream stream = mmf.CreateViewStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            writer.Write(bytes, 0, bytes.Length);
        }
    }

    public void ReadFromMemoryMappedFile(string mapName, int bytesNumber, out byte[] frame)
    {
        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mapName))
        using (MemoryMappedViewStream stream = mmf.CreateViewStream())
        using (BinaryReader reader = new BinaryReader(stream))
        {
            frame = reader.ReadBytes(bytesNumber);
        }
    }
}
