using UnityEngine;
using TMPro;

public class ReadableNote : MonoBehaviour
{
    public GameObject notePrefab;
    public GameObject player;

    public string titleText;
    public string contentText;

    private GameObject instantiatedNote;
    private GameObject pointer;
    private TextMeshProUGUI option1;
    private TextMeshProUGUI option2;
    private TextMeshProUGUI title;
    private TextMeshProUGUI content;

    private bool openedNote = false;
    private int page;
    private int pageCount;
    private int pointedOption;
    private bool ableToChoose;
    private int updateCount;
    private int framesToWait = 2;

    void KeysListener()
    {
        //Test by pressing n
        if (Input.GetKeyDown(KeyCode.N) && !openedNote)
        {
            OpenNote();
        }
            
        //Choose option continue, back or close
        if (Input.GetKeyDown(KeyCode.Space) && openedNote && updateCount == framesToWait)
        {
            if (pointedOption == 1 && page < pageCount)
            {
                page++;
                DisplayPage(page);
            }
            else if (pointedOption == 2 && page > 1)
            {
                page--;
                DisplayPage(page);
            }
            else if (pointedOption == 1 && page == pageCount)
            {
                CloseNote();
            }
        }

        //Point option 1 or 2
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) ) && ableToChoose && openedNote)
        {
            if (pointedOption == 1)
            {
                PointOption(option2);
            }
            else
            {
                PointOption(option1);
            }
        }
    }
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
                DisplayPage(page);
            }
        }
    }
    void OpenNote()
    {
        //Instatiate note
        instantiatedNote = Instantiate(notePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

        //Disable player movement
        player.GetComponent<AdvanceTestMovement>().enabled = false;

        //Get TextMeshProUGUIs
        option1 = instantiatedNote.transform.Find("Options").Find("1").gameObject.GetComponent<TextMeshProUGUI>();
        option2 = instantiatedNote.transform.Find("Options").Find("2").gameObject.GetComponent<TextMeshProUGUI>();
        title = instantiatedNote.transform.Find("Content").Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        content = instantiatedNote.transform.Find("Content").Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        pointer = instantiatedNote.transform.Find("Options").Find("Pointer").gameObject;

        //Set proper values
        title.text = titleText;
        content.text = contentText;
        page = 1;
        updateCount = 0;
        openedNote = true;
    }

    void DisplayPage(int page)
    {
        //Display page of content text
        content.pageToDisplay = page;

        //Display proper buttons
        if (page == 1 && page != pageCount)
        {
            PointOption(option1);
            ableToChoose = false;
            option1.text = "Continue";
            option2.gameObject.SetActive(false);
        }
        else if (page == 1 && page == pageCount)
        {
            PointOption(option1);
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
    }

    void CloseNote()
    {
        Destroy(instantiatedNote);

        //Enable player movement
        player.GetComponent<AdvanceTestMovement>().enabled = true;
        openedNote = false;
    }

    void PointOption(TextMeshProUGUI option)
    {
        //Change color of all options to lightGrey (118, 118, 118)
        option1.color = new Color(0.4625f, 0.4625f, 0.4625f);
        option2.color = new Color(0.4625f, 0.4625f, 0.4625f);

        //Change color of pointed option to white (255, 255, 255)
        option.color = new Color(1f, 1f, 1f);

        //Set position of pointer to pointed option
        pointer.transform.localPosition =
            new Vector3(pointer.transform.localPosition.x, option.transform.localPosition.y, pointer.transform.localPosition.z);

        if (option == option1) pointedOption = 1;
        else pointedOption = 2;
    }
}