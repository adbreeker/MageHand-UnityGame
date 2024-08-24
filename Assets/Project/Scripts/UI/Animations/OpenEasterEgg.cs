using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;

public class OpenEasterEgg : MonoBehaviour
{
    public List<GameObject> randomDialoguesToSpawn = new List<GameObject>();

    public RawImage blackoutImage;
    public RawImage topBlackoutImage;
    [Header("Canvas Groups of Texts")]
    public CanvasGroup text1;
    public CanvasGroup text2;
    public CanvasGroup continueText;

    float fadeOutSpeed = 0.025f;
    private bool animationEnded = false;
    private bool skip = false;

    AudioManager AudioManager => GameParams.Managers.audioManager;
    private void Start()
    {
        StartCoroutine(EasterEgg());
    }

    private void Update()
    {
        if (animationEnded && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RestartScene());
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.UI_SelectOption);
            animationEnded = false;
        }
        if (Input.GetKeyDown(KeyCode.Space)) skip = true;
    }

    IEnumerator EasterEgg()
    {
        //deactivate other dialogues
        foreach(OpenDialogue openDialogue in FindObjectsOfType<OpenDialogue>())
        {
            openDialogue.gameObject.SetActive(false);
        }
        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            if (audioSource.gameObject.name.StartsWith("VOICES_"))
            {
                Destroy(audioSource.gameObject);
            }
        }
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);
        PlayerParams.Controllers.inventory.ableToInteract = true;
        PlayerParams.Controllers.spellbook.ableToInteract = true;
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
        PlayerParams.Controllers.spellsMenu.ableToInteract = true;
        PlayerParams.Controllers.journal.ableToInteract = true;
        
        //activate dialogue
        yield return new WaitForSeconds(2.5f);
        GameObject spawnedDialogue = SpawnDialogueOnPlayer.SpawnDialogue(randomDialoguesToSpawn[UnityEngine.Random.Range(0, randomDialoguesToSpawn.Count)]);

        //teleport
        yield return new WaitForSeconds(5f);
        PlayerParams.Controllers.playerMovement.TeleportTo(new Vector3(6000, 0, 6000), PlayerParams.Objects.player.transform.rotation.eulerAngles.y ,Color.blue);


        //blackout picture
        yield return new WaitForSeconds(0.4f);

        float alpha = 0;

        StartCoroutine(AudioManager.FadeOutBus(FmodBuses.SFX, fadeOutSpeed));
        StartCoroutine(AudioManager.FadeOutBus(FmodBuses.Music, fadeOutSpeed));

        while (alpha < 1)
        {
            alpha += fadeOutSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }

        Destroy(AudioManager.backgroundMusic.gameObject);

        if (PlayerParams.Controllers.pauseMenu != null) PlayerParams.Controllers.pauseMenu.freezeTime = false;
        yield return new WaitForFixedUpdate();

        //turn off movement and ui 
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.spellbook.CloseSpellbook();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.spellsMenu.CloseMenu();
        PlayerParams.Controllers.journal.CloseJournal();

        PlayerParams.Controllers.inventory.ableToInteract = false;
        PlayerParams.Controllers.spellbook.ableToInteract = false;
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Controllers.spellsMenu.ableToInteract = false;
        PlayerParams.Controllers.journal.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        spawnedDialogue.SetActive(false);

        //show texts if not on c1l1
        yield return new WaitForSeconds(2.5f);
        if (SceneManager.GetActiveScene().name != "Chapter_1_Level_1")
        {
            skip = false;
            //show text1
            alpha = 0;
            while (alpha < 1)
            {
                alpha += 0.005f;
                text1.alpha = alpha;

                if (!skip) yield return new WaitForSeconds(0);
            }
            if (!skip) yield return new WaitForSeconds(0.5f);
            skip = false;
            //show text2
            alpha = 0;
            while (alpha < 1)
            {
                alpha += 0.005f;
                text2.alpha = alpha;

                if (!skip) yield return new WaitForSeconds(0);
            }
            if (!skip) yield return new WaitForSeconds(0.5f);
            skip = false;
            //show continue text
            alpha = 0;
            while (alpha < 1)
            {
                alpha += 0.01f;
                continueText.alpha = alpha;

                if (!skip) yield return new WaitForSeconds(0);
            }
            animationEnded = true;
        }
        //if on c1l1 restart scene
        else
        {
            StartCoroutine(RestartScene());
        }
    }

    IEnumerator RestartScene()
    {
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += fadeOutSpeed;
            topBlackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
