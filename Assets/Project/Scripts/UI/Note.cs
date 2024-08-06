using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public string titleText;
    public string contentText;

    private GameObject pointer;
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    public GameObject scrollView;

    private AudioSource openSound;
    private AudioSource closeSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

    private bool openedNote = false;
    private bool fromJournal = false;
    public bool saveToJournal = true;

    private float keyTimeDelayer = 0;
    private float scrollSpeed;

    private void Update()
    {
        KeysListener();
        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
    }

    void KeysListener()
    {
        //Choose option continue, back or close
        if (Input.GetKeyDown(KeyCode.Space) && openedNote)
        {
            selectSound.Play();
            CloseNote();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && openedNote && fromJournal)
        {
            closeSound.Play();
            CloseNote();
        }

        if (openedNote && Input.GetKey(KeyCode.W) && scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition < 1)
        {
            scrollSpeed = 750 * Time.unscaledDeltaTime /
                (scrollView.GetComponent<ScrollRect>().content.sizeDelta.y
                - scrollView.GetComponent<RectTransform>().sizeDelta.y);
            scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += scrollSpeed;
        }

        if (openedNote && Input.GetKey(KeyCode.S) && scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition > 0)
        {
            scrollSpeed = 750 * Time.unscaledDeltaTime /
                (scrollView.GetComponent<ScrollRect>().content.sizeDelta.y
                - scrollView.GetComponent<RectTransform>().sizeDelta.y);
            scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -scrollSpeed;
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

        openSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_Open);
        closeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        openSound.Play();

        //Set proper values
        title.text = titleText;
        content.text = contentText;
        openedNote = true;
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
            if(saveToJournal)
            {
                if (!PlayerParams.Controllers.journal.notesJournal.ContainsKey(titleText))
                    PlayerParams.Controllers.journal.notesJournal.Add(titleText, contentText);
            }
        }
        else PlayerParams.Controllers.journal.DisplayNamesBack();

        openedNote = false;
    }
}