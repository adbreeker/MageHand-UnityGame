using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.IO.Pipes;
using System;
using FMODUnity;
using FMOD.Studio;


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

    [Header("Pop up options:")]
    public GameObject popUpPrefab;
    public float timeToFadeOutPopUp = 1;
    public float timeOfFadingOutPopUp = 0.007f;

    [Header("Spells Prefabs:")]
    public GameObject lightPrefab;
    public GameObject firePrefab;
    public GameObject markPrefab;
    public GameObject collectEffectPrefab;
    public GameObject openEffectPrefab;
    public GameObject slowEffectPrefab;

    [Header("Casting effect prefab")]
    public GameObject castingEffectPrefab;

    [Header("Easter Egg")]
    public GameObject openEasterEggPrefab;
    private GameObject instantiatedOpenEasterEggPrefab;

    //private bool isTranscribing = false;

    FmodEvents FmodEvents => GameParams.Managers.fmodEvents;

    private bool _isAbleToCast = true;

    private void Start()
    {
        PlayerParams.Variables.startingManaRegen = manaRegen;
    }

    private void Update()
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
            mana -= scroll.manaCost;

            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFinished);
            currentSpell = "Light";
            PlayerParams.Controllers.handInteractions.AddToHand(Instantiate(lightPrefab), true, true);
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);
        }
    }

    public IEnumerator CollectSpell() //casting pick up spell
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Collect");
        if(scroll != null)
        {
            mana -= scroll.manaCost;

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
                yield return new WaitForEndOfFrame();
                if(nearestItem != null) 
                {
                    GameObject collectEffect = Instantiate(collectEffectPrefab, nearestItem.transform);
                    while (nearestItem.transform.position != PlayerParams.Controllers.handInteractions.holdingPoint.position)
                    {
                        nearestItem.transform.position = Vector3.MoveTowards(nearestItem.transform.position, PlayerParams.Controllers.handInteractions.holdingPoint.position, 7.0f * Time.fixedDeltaTime);
                        yield return new WaitForFixedUpdate();
                    }
                    nearestItem.layer = LayerMask.NameToLayer("Item");
                    PlayerParams.Controllers.handInteractions.AddToHand(nearestItem, true, false);
                    Destroy(collectEffect);
                }
            }
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);
        }
    }

    public void FireSpell() //casting fire spell
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Fire");
        if (scroll != null)
        {
            mana -= scroll.manaCost;

            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFinished);
            currentSpell = "Fire";
            PlayerParams.Controllers.handInteractions.AddToHand(Instantiate(firePrefab), true, true);
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);
        }
    }

    public void SpeakSpell()
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Speak");
        if (scroll != null && !PlayerParams.Controllers.playerMovement.isMoving)
        {
            SkullPlatformBehavior skullPlatform = null;
            foreach(RaycastHit potentialPlatform in Physics.RaycastAll(PlayerParams.Objects.player.transform.position, Vector3.down, 5.0f))
            {
                if(potentialPlatform.collider.GetComponent<SkullPlatformBehavior>() != null)
                {
                    skullPlatform = potentialPlatform.collider.GetComponent<SkullPlatformBehavior>();
                    break;
                }
            }

            if (skullPlatform != null && skullPlatform.platformActive)
            {
                mana -= scroll.manaCost;

                RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFinished);
                currentSpell = "Speak";
                skullPlatform.UsePlatform();
            }
            else
            {
                RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);
            }
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);
        }
    }

    public void MarkSpell() //marking place under player for future teleportation
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Mark");
        if (scroll != null)
        {
            mana -= scroll.manaCost;

            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFinished);
            currentSpell = "Mark";
            PlayerParams.Controllers.handInteractions.AddToHand(Instantiate(markPrefab), true, true);
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);
        }
    }

    public IEnumerator SlowSpell()
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Slow");
        if (scroll != null)
        {
            mana -= scroll.manaCost;

            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFinished);
            GameObject slowEffect = Instantiate(slowEffectPrefab, PlayerParams.Objects.player.transform);

            float slowValue = 0.1f;

            GameParams.Variables.currentTimeScale = slowValue;
            Time.timeScale = slowValue;

            yield return new WaitForSeconds(10f * slowValue);
            GameParams.Variables.currentTimeScale = 1.0f;

            Destroy(slowEffect);
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);
        }
    }

    public void OpenSpell() //casting break in spell - occurs in tutorial only
    {
        SpellScrollInfo scroll = spellbook.GetSpellInfo("Open");
        if (scroll != null)
        {
            mana -= scroll.manaCost;

            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFinished);
            //^ instead of this
            //RuntimeManager.PlayOneShot(FmodEvents.SFX_OpenSpellActivation);
            Vector3 castPos = PlayerParams.Objects.player.transform.position;
            Collider[] nearObjects = Physics.OverlapSphere(castPos, 2.0f, LayerMask.GetMask("Default"));
            foreach(Collider potentialLock in nearObjects) 
            {
                if(potentialLock.tag == "Lock")
                {
                    Instantiate(openEffectPrefab, potentialLock.transform).transform.SetParent(potentialLock.transform.parent);
                    potentialLock.GetComponent<LockBehavior>().OpenLock();
                }
            }
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);

            if (instantiatedOpenEasterEggPrefab == null)
            {
                instantiatedOpenEasterEggPrefab = Instantiate(openEasterEggPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            }
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
        PlayerParams.Controllers.HUD.SpawnPopUp("Cast a Spell.", timeToFadeOutPopUp, timeOfFadingOutPopUp);

        // Effects
        GameObject castingEffect = Instantiate(castingEffectPrefab, hand);
        EventInstance castingSound = GameParams.Managers.audioManager.PlayOneShotReturnInstance(FmodEvents.SFX_CastingSpell);

        // Wait for the specified recording time
        yield return new WaitForSecondsRealtime(2f);
        castingEffect.GetComponent<ParticlesColor>().ChangeColorOfEffect(new Color(0f, 1f, 0.5f));

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

        if (word.Length >= 4 && word.Substring(0, 4) == "None")
        {
            PlayerParams.Controllers.HUD.SpawnPopUp("Casting word:<br>(silence)", timeToFadeOutPopUp, timeOfFadingOutPopUp, false);
        }
        else { PlayerParams.Controllers.HUD.SpawnPopUp("Casting word:<br>" + word, timeToFadeOutPopUp, timeOfFadingOutPopUp, false); }

        castingSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Destroy(castingEffect);

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
            StartCoroutine(CollectSpell());
        }
        else if (NormalizeTranscribedText(name) == "fire")
        {
            FireSpell();
        }
        else if (NormalizeTranscribedText(name) == "speak")
        {
            SpeakSpell();
        }
        else if (NormalizeTranscribedText(name) == "mark")
        {
            MarkSpell();
        }
        else if (NormalizeTranscribedText(name) == "slow")
        {
            StartCoroutine(SlowSpell());
        }
        else if (NormalizeTranscribedText(name) == "open")
        {
            OpenSpell();
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.SFX_CastingSpellFailed);
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
