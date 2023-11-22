using UnityEngine;
using TMPro;
using System.Collections;

public class Note : MonoBehaviour
{
    public string titleText;
    public string contentText;

    private GameObject pointer;
    private TextMeshProUGUI option1;
    private TextMeshProUGUI option2;
    private TextMeshProUGUI title;
    private TextMeshProUGUI content;

    private AudioSource openSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

    private bool openedNote = false;
    private int page;
    private int pageCount;
    private int pointedOption;
    private bool ableToChoose;
    private bool fromJournal = false;
    private int updateCount = 0;
    private int framesToWait = 1;

    private int keyTimeDelayFirst = 20;
    private int keyTimeDelay = 10;
    private int keyTimeDelayer = 0;

    private void Update()
    {
        KeysListener();


        //Display first page after few frames to load everything, so pageCount works properly
        if (openedNote && updateCount < framesToWait)
        {
            updateCount++;
            if (updateCount == framesToWait)
            {
                pageCount = content.textInfo.pageCount;
                option1.gameObject.SetActive(true);
                StartCoroutine(DisplayPage(page, 1));
            }
        }

        if (keyTimeDelayer > 0)
        {
            keyTimeDelayer--;
        }
    }

    void KeysListener()
    {
        //Choose option continue, back or close
        if (Input.GetKeyDown(KeyCode.Space) && openedNote && updateCount == framesToWait)
        {
            selectSound.Play();
            if (pointedOption == 1 && page < pageCount)
            {
                page++;
                StartCoroutine(DisplayPage(page, 1));
            }
            else if (pointedOption == 2 && page > 1)
            {
                page--;
                StartCoroutine(DisplayPage(page, 2));
            }
            else if (pointedOption == 1 && page == pageCount)
            {
                CloseNote();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && openedNote && fromJournal && updateCount == framesToWait)
        {
            CloseNote();
        }

        //Point option 1 or 2
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && ableToChoose && openedNote)
        {
            if (pointedOption == 1)
            {
                changeSound.Play();
                PointOption(option2);
            }
            else
            {
                changeSound.Play();
                PointOption(option1);
            }
            keyTimeDelayer = keyTimeDelayFirst;
        }

        if (keyTimeDelayer == 0 && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && ableToChoose && openedNote)
        {
            if (pointedOption == 1)
            {
                changeSound.Play();
                PointOption(option2);
            }
            else
            {
                changeSound.Play();
                PointOption(option1);
            }
            keyTimeDelayer = keyTimeDelay;
        }
    }
    public void OpenNote(bool givenFromJournal = false)
    {
        fromJournal = givenFromJournal;

        //Disable other controls (close first, because it activates movement and enable other ui)
        if (!fromJournal)
        {
            PlayerParams.Controllers.inventory.CloseInventory();
            PlayerParams.Controllers.spellbook.CloseSpellbook();
            PlayerParams.Controllers.journal.CloseJournal();
            PlayerParams.Controllers.pauseMenu.CloseMenu();

            PlayerParams.Controllers.inventory.ableToInteract = false;
            PlayerParams.Controllers.spellbook.ableToInteract = false;
            PlayerParams.Controllers.journal.ableToInteract = false;
            PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        }
        else transform.Find("Background").Find("BlackoutBackground").gameObject.SetActive(false);


        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        openSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Open);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        //Get TextMeshProUGUIs
        option1 = transform.Find("Options").Find("1").gameObject.GetComponent<TextMeshProUGUI>();
        option2 = transform.Find("Options").Find("2").gameObject.GetComponent<TextMeshProUGUI>();
        title = transform.Find("Content").Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        content = transform.Find("Content").Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        pointer = transform.Find("Options").Find("1").Find("Pointer").gameObject;

        openSound.Play();

        //Set proper values
        title.text = titleText;
        content.text = contentText;
        page = 1;
        updateCount = 0;
        openedNote = true;
    }

    IEnumerator DisplayPage(int page, int optionsToPoint)
    {
        //Display page of content text
        content.pageToDisplay = page;

        //Display proper buttons
        if (page == 1 && page != pageCount)
        {
            ableToChoose = false;
            option1.text = "Continue";
            option2.gameObject.SetActive(false);
        }
        else if (page == 1 && page == pageCount)
        {
            ableToChoose = false;
            option1.text = "Close";
            option2.gameObject.SetActive(false);
        }
        else if (page > 1 && page < pageCount)
        {
            ableToChoose = true;
            option1.text = "Continue";
            option2.gameObject.SetActive(true);
        }
        else if (page == pageCount)
        {
            ableToChoose = true;
            option1.text = "Close";
            option2.gameObject.SetActive(true);
        }

        //Wait for text to change size
        yield return new WaitForSeconds(0);

        if (optionsToPoint == 2 && option2.gameObject.activeSelf == true)
        {
            PointOption(option2);
        }
        else PointOption(option1);
    }

    void CloseNote()
    {
        Destroy(openSound.gameObject, openSound.clip.length);
        Destroy(changeSound.gameObject, changeSound.clip.length);
        Destroy(selectSound.gameObject, selectSound.clip.length);
        Destroy(gameObject);

        //Enable other controls
        if (!fromJournal)
        {
            PlayerParams.Controllers.inventory.ableToInteract = true;
            PlayerParams.Controllers.spellbook.ableToInteract = true;
            PlayerParams.Controllers.pauseMenu.ableToInteract = true;
            PlayerParams.Controllers.journal.ableToInteract = true;
            PlayerParams.Variables.uiActive = false;
            PlayerParams.Objects.hand.SetActive(true);
            if (!PlayerParams.Controllers.journal.notesJournal.ContainsKey(titleText))
                PlayerParams.Controllers.journal.notesJournal.Add(titleText, contentText);
        }
        else PlayerParams.Controllers.journal.DisplayNamesBack();

        openedNote = false;
    }

    void PointOption(TextMeshProUGUI option)
    {
        //Change color of all options to darkGrey (68, 68, 68)
        option1.color = new Color(0.2666f, 0.2666f, 0.2666f);
        option2.color = new Color(0.2666f, 0.2666f, 0.2666f);

        //Change color of pointed option to white (255, 255, 255)
        option.color = new Color(1f, 1f, 1f);

        //Set position of pointer to pointed option
        pointer.transform.SetParent(option.transform);

        if (option == option1) pointedOption = 1;
        else pointedOption = 2;
    }
}