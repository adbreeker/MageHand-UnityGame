using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    [Header("Content text")]
    public string nameText;
    public bool guideVoiceline = true;
    [TextArea(4, 4)]
    public string contentText;
    [Space(20)]
    [Header("Option 1")]
    [TextArea(2, 2)]
    public string option1Text;
    public Canvas option1Choice;
    public int option1Points = 0;
    public UnityEvent option1Action;
    [Header("Option 2")]
    [TextArea(2, 2)]
    public string option2Text;
    public Canvas option2Choice;
    public int option2Points = 0;
    public UnityEvent option2Action;
    [Header("Option 3")]
    [TextArea(2, 2)]
    public string option3Text;
    public Canvas option3Choice;
    public int option3Points = 0;
    public UnityEvent option3Action;
    [Header("Option 4")]
    [TextArea(2, 2)]
    public string option4Text;
    public Canvas option4Choice;
    public int option4Points = 0;
    public UnityEvent option4Action;
    [Space(20)]
    [Header("Player choice")]
    public string playerChoice = "";

    private GameObject pointer;
    private TextMeshProUGUI nameTextObject;
    private TextMeshProUGUI contentTextObject;
    private float textSpeed;
    private Dictionary<int, TextMeshProUGUI> options;
    private Dictionary<int, Canvas> optionsChoices;
    private Dictionary<int, string> optionsTexts;
    private Dictionary<int, int> optionsPoints;
    private Dictionary<int, UnityEvent> optionsActions;

    private bool listen = false;
    private bool skip = false;
    private int choice;

    private EventReference voiceRef;
    private EventInstance voiceInstance;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    private FmodEvents FmodEvents => GameParams.Managers.fmodEvents;
    private AudioManager AudioManager => GameParams.Managers.audioManager;
    void Start()
    {
        textSpeed = transform.parent.GetComponent<OpenDialogue>().textSpeed;

        //Disable other controls (close first, because it activates movement and enable other ui)
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
        PlayerParams.Objects.hand.SetActive(false);

        pointer = transform.Find("Options").Find("Pointer").gameObject;
        nameTextObject = transform.Find("Text").Find("Name").GetComponent<TextMeshProUGUI>();
        contentTextObject = transform.Find("Text").Find("Content").GetComponent<TextMeshProUGUI>();

        if (guideVoiceline) voiceRef = FmodEvents.SFX_VoiceGuide;
        else voiceRef = FmodEvents.SFX_VoiceAlandos;

        //Create dicts of options, choices when options are chosen and text of options (indexed 1-4)
        options = new Dictionary<int, TextMeshProUGUI>();
        options.Add(1, transform.Find("Options").Find("1").GetComponent<TextMeshProUGUI>());
        options.Add(2, transform.Find("Options").Find("2").GetComponent<TextMeshProUGUI>());
        options.Add(3, transform.Find("Options").Find("3").GetComponent<TextMeshProUGUI>());
        options.Add(4, transform.Find("Options").Find("4").GetComponent<TextMeshProUGUI>());
        optionsChoices = new Dictionary<int, Canvas>();
        optionsChoices.Add(1, option1Choice);
        optionsChoices.Add(2, option2Choice);
        optionsChoices.Add(3, option3Choice);
        optionsChoices.Add(4, option4Choice);
        optionsTexts = new Dictionary<int, string>();
        optionsTexts.Add(1, option1Text);
        optionsTexts.Add(2, option2Text);
        optionsTexts.Add(3, option3Text);
        optionsTexts.Add(4, option4Text);
        optionsPoints = new Dictionary<int, int>();
        optionsPoints.Add(1, option1Points);
        optionsPoints.Add(2, option2Points);
        optionsPoints.Add(3, option3Points);
        optionsPoints.Add(4, option4Points);
        optionsActions = new Dictionary<int, UnityEvent>();
        optionsActions.Add(1, option1Action);
        optionsActions.Add(2, option2Action);
        optionsActions.Add(3, option3Action);
        optionsActions.Add(4, option4Action);

        //Type text
        choice = 1;
        StartCoroutine(TypeText());
    }
    void Update()
    {
        //Listen if typing text is done
        if (listen) KeysListener();
        else KeysListenerSkip();

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
    }

    private void OnDisable()
    {
        if (voiceInstance.isValid()) voiceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    void PointOption(TextMeshProUGUI option)
    {
        //Change color of all options to darkGrey (68, 68, 68)
        foreach (int key in options.Keys)
        {
            options[key].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        //Change color of pointed option to white (255, 255, 255)
        option.color = new Color(1f, 1f, 1f);

        //Set position of pointer to pointed option
        pointer.transform.localPosition =
            new Vector3(pointer.transform.localPosition.x, option.transform.localPosition.y, pointer.transform.localPosition.z);
    }

    void KeysListener()
    {
        //Change pointed option up
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (choice == 1)
            {
                for (int i = 4; i > 0; i--)
                {
                    if (!string.IsNullOrWhiteSpace(options[i].text))
                    {
                        choice = i;
                        if(choice != 1) RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                        PointOption(options[choice]);
                        break;
                    }
                }
            }
            else
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                choice--;
                PointOption(options[choice]);
            }
            keyTimeDelayer = keyTimeDelayFirst;
        }

        //Change pointed option down
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (choice == 4)
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                choice = 1;
                PointOption(options[choice]); 
            }
            else if (string.IsNullOrWhiteSpace(options[choice + 1].text))
            {
                if(choice != 1) RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                choice = 1;
                PointOption(options[choice]);
            }
            else
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                choice++;
                PointOption(options[choice]);
            }
            keyTimeDelayer = keyTimeDelayFirst;
        }

        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.W))
        {
            if (choice == 1)
            {
                for (int i = 4; i > 0; i--)
                {
                    if (!string.IsNullOrWhiteSpace(options[i].text))
                    {
                        choice = i;
                        if (choice != 1) RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                        PointOption(options[choice]);
                        break;
                    }
                }
            }
            else
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                choice--;
                PointOption(options[choice]);
            }
            keyTimeDelayer = keyTimeDelay;
        }

        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.S))
        {

            if (choice == 4)
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                choice = 1;
                PointOption(options[choice]);
            }
            else if (string.IsNullOrWhiteSpace(options[choice + 1].text))
            {
                if(choice != 1) RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                choice = 1;
                PointOption(options[choice]);
            }
            else
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                choice++;
                PointOption(options[choice]);
            }
            keyTimeDelayer = keyTimeDelay;
        }

        //Choose pointed option (if choice is null, end dialogue and activate other controls)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RuntimeManager.PlayOneShot(FmodEvents.NP_UiSelectOption);

            //Save dialogue to player's diary
            if (transform.parent.GetComponent<OpenDialogue>().saveDialogue)
            {
                PlayerParams.Controllers.journal.dialoguesJournal[transform.parent.GetComponent<OpenDialogue>().dialogueSaveName].Add(new List<string> { nameText, contentText });
                if (optionsTexts[choice] != "(Continue.)" && optionsTexts[choice] != "(End dialogue.)")
                {
                    string textToSave = optionsTexts[choice].Replace(" (End dialogue.)", "");
                    PlayerParams.Controllers.journal.dialoguesJournal[transform.parent.GetComponent<OpenDialogue>().dialogueSaveName].Add(new List<string> { "You", textToSave });
                }
            }

            playerChoice = optionsTexts[choice];
            PlayerParams.Controllers.pointsManager.plotPoints += optionsPoints[choice];
            optionsActions[choice]?.Invoke();
            gameObject.SetActive(false);
            if (optionsChoices[choice] == null)
            {
                //Enable other controls
                PlayerParams.Variables.uiActive = false;
                PlayerParams.Objects.hand.SetActive(true);
                PlayerParams.Controllers.inventory.ableToInteract = true;
                PlayerParams.Controllers.spellbook.ableToInteract = true;
                PlayerParams.Controllers.pauseMenu.ableToInteract = true;
                PlayerParams.Controllers.spellsMenu.ableToInteract = true;
                PlayerParams.Controllers.journal.ableToInteract = true;
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                optionsChoices[choice].gameObject.SetActive(true);
            }
        }
    }

    void KeysListenerSkip()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skip = true;
        }
    }

    IEnumerator TypeText()
    {
        //If there is no speaker name, expand space for content, else, show speaker name 
        if (string.IsNullOrWhiteSpace(nameText))
        {
            nameTextObject.gameObject.SetActive(false);
            contentTextObject.GetComponent<RectTransform>().sizeDelta =
                new Vector2(contentTextObject.GetComponent<RectTransform>().sizeDelta.x, 200f);
        }
        else
        {
            nameTextObject.text = nameText;
            voiceInstance = AudioManager.PlayOneShotReturnInstance(voiceRef);
        }

        //Set position of pointer to first option
        pointer.transform.localPosition =
            new Vector3(pointer.transform.localPosition.x, options[1].transform.localPosition.y, pointer.transform.localPosition.z); //y was + 4f

        //Set color of first option to white (255, 255, 255)
        options[1].color = new Color(1f, 1f, 1f);

        //Type content
        contentTextObject.text = string.Empty;
        foreach (char c in contentText.ToCharArray())
        {
            contentTextObject.text += c;
            if (!skip) yield return new WaitForSeconds(textSpeed);

            //Start to fade out voice
            if (contentTextObject.text.Length >= contentText.Length - 5)
            {
                StartCoroutine(FadeOutVoice(0.5f));
            }
        }
        //Show pointer
        pointer.gameObject.SetActive(true);
        if (!skip) yield return new WaitForSeconds(textSpeed);

        //Type options
        foreach (int key in options.Keys)
        {
            if (string.IsNullOrWhiteSpace(optionsTexts[key]))
            {
                options[key].text = optionsTexts[key];
            }
            else
            {
                options[key].text = string.Empty;
                options[key].gameObject.SetActive(true);
                foreach (char c in optionsTexts[key].ToCharArray())
                {
                    options[key].text += c;
                    if (!skip) yield return new WaitForSeconds(textSpeed/2);
                }
            }
        }

        //Activate KeysListener
        listen = true;
    }

    IEnumerator FadeOutVoice(float speed)
    {
        if (voiceInstance.isValid())
        {
            voiceInstance.getVolume(out float currentVolume);
            float startVolume = currentVolume;

            while (voiceInstance.isValid() && currentVolume > 0)
            {
                AudioManager.SetInstanceVolume(voiceInstance, currentVolume - startVolume * Time.deltaTime / speed);
                if (!skip) yield return new WaitForSeconds(textSpeed);
                voiceInstance.getVolume(out currentVolume);
            }

            if (voiceInstance.isValid()) voiceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
