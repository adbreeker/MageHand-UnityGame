using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Content text")]
    public string nameText;
    [TextArea(3, 10)]
    public string contentText;

    [Header("Options text")]
    [TextArea(1, 2)]
    public string option1Text;
    [TextArea(1, 2)]
    public string option2Text;
    [TextArea(1, 2)]
    public string option3Text;
    [TextArea(1, 2)]
    public string option4Text;

    [Header("Choices canvases")]
    public Canvas option1Choice;
    public Canvas option2Choice;
    public Canvas option3Choice;
    public Canvas option4Choice;

    [Header("Prefab objects")]
    public GameObject pointer;
    public Canvas dialogueCanvas;
    public TextMeshProUGUI nameTextObject;
    public TextMeshProUGUI contentTextObject;
    public TextMeshProUGUI option1;
    public TextMeshProUGUI option2;
    public TextMeshProUGUI option3;
    public TextMeshProUGUI option4;

    [Header("Sound of voice")]
    public AudioSource voice;

    private float textSpeed;
    private Dictionary<int, TextMeshProUGUI> options;
    private Dictionary<int, Canvas> optionsChoices;
    private Dictionary<int, string> optionsTexts;

    private bool listen = false;
    private bool skip = false;
    private int choice;

    void Start()
    {
        textSpeed = transform.parent.GetComponent<OpenDialogue>().textSpeed;

        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.spellbook.CloseSpellbook();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.inventory.ableToInteract = false;
        PlayerParams.Controllers.spellbook.ableToInteract = false;
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        //Create dicts of options, choices when options are chosen and text of options (indexed 1-4)
        options = new Dictionary<int, TextMeshProUGUI>();
        options.Add(1, option1);
        options.Add(2, option2);
        options.Add(3, option3);
        options.Add(4, option4);
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

        //Type text
        choice = 1;
        StartCoroutine(TypeText());
    }
    void Update()
    {
        //Listen if typing text is done
        if (listen) KeysListener();
        else KeysListenerSkip();
    }

    void PointOption(TextMeshProUGUI option)
    {
        //Change color of all options to lightGrey (118, 118, 118)
        //foreach (int key in options.Keys)
        //{
        //    options[key].color = new Color(0.4625f, 0.4625f, 0.4625f);
        //}

        //Change color of all options to darkGrey (68, 68, 68)
        foreach (int key in options.Keys)
        {
            options[key].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        //Change color of pointed option to white (255, 255, 255)
        option.color = new Color(1f, 1f, 1f);

        //Set position of pointer to pointed option
        pointer.transform.localPosition =
            new Vector3(pointer.transform.localPosition.x, option.transform.localPosition.y, pointer.transform.localPosition.z); //y was + 4f
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
                        PointOption(options[choice]);
                        break;
                    }
                }
            }
            else
            {
                choice--;
                PointOption(options[choice]);
            }
        }

        //Change pointed option down
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (choice == 4)
            {
                choice = 1;
                PointOption(options[choice]); 
            }
            else if (string.IsNullOrWhiteSpace(options[choice + 1].text))
            {
                choice = 1;
                PointOption(options[choice]);
            }
            else
            {
                choice++;
                PointOption(options[choice]);
            }
        }

        //Choose pointed option (if choice is null, end dialogue and activate other controls)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Save dialogue to player's diary
            //transform.parent.GetComponent<OpenDialogue>().dialogueName;
            //PlayerParams.Controllers.dialogueDiary.dialogueDiary.Add(new List<string> { nameText, contentText });
            //PlayerParams.Controllers.dialogueDiary.dialogueDiary.Add(new List<string> { "You ", optionsTexts[choice] });
            PlayerParams.Controllers.dialogueDiary.dialogueDiary[transform.parent.GetComponent<OpenDialogue>().dialogueName].Add(new List<string> { nameText, contentText });
            PlayerParams.Controllers.dialogueDiary.dialogueDiary[transform.parent.GetComponent<OpenDialogue>().dialogueName].Add(new List<string> { "You ", optionsTexts[choice] });
            foreach (string key in PlayerParams.Controllers.dialogueDiary.dialogueDiary.Keys)
            {
                foreach (List<string> list in PlayerParams.Controllers.dialogueDiary.dialogueDiary[key])
                {
                    foreach(string text in list)
                    {
                        Debug.Log(text);
                    }
                }
            }

            if (optionsChoices[choice] == null)
            {
                dialogueCanvas.gameObject.SetActive(false);

                //Enable other controls
                PlayerParams.Variables.uiActive = false;
                PlayerParams.Objects.hand.SetActive(true);
                PlayerParams.Controllers.inventory.ableToInteract = true;
                PlayerParams.Controllers.spellbook.ableToInteract = true;
                PlayerParams.Controllers.pauseMenu.ableToInteract = true;
            }
            else
            {
                dialogueCanvas.gameObject.SetActive(false);
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
            voice.Play();
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
        float startVolume = voice.volume;

        while (voice.volume > 0)
        {
            voice.volume -= startVolume * Time.deltaTime / speed;

            if (!skip) yield return new WaitForSeconds(textSpeed);
        }

        voice.Stop();
        voice.volume = startVolume;
    }
}
