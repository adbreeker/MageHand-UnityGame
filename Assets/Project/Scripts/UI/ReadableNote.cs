using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadableNote : MonoBehaviour
{
    public GameObject notePrefab;
    public GameObject player;

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

    void KeysListener()
    {
        //Choose option
        if (Input.GetKeyDown(KeyCode.Space) && openedNote)
        {
            if (pointedOption == 1 && page < pageCount)
            {
                page++;
                DisplayPage(page);
            }
            if (pointedOption == 2 && page > 1)
            {
                page--;
                DisplayPage(page);
            }
            if (pointedOption == 1 && page == pageCount)
            {
                CloseNote();
            }
        }

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

    private void Start()
    {
        OpenNote();
    }

    private void Update()
    {
        KeysListener();
    }

    void OpenNote()
    {
        Debug.Log("opennote ");
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

        pageCount = content.textInfo.pageCount;

        page = 1;

        DisplayPage(page);
        PointOption(option1);
        openedNote = true;
        Debug.Log("pagecount " + pageCount);
    }

    void DisplayPage(int page)
    {
        Debug.Log("displaypage " + page);
        ableToChoose = false;
        option2.gameObject.SetActive(false);
        content.pageToDisplay = page;
        if (page < pageCount && page > 1)
        {
            ableToChoose = true;
            option1.gameObject.SetActive(true);
        }
        else if(page == 1)
        {
            option1.text = "Continue";
        }
        else if (page == pageCount)
        {
            option1.text = "Close";
        }
    }

    void CloseNote()
    {
        Debug.Log("closenote");
        Destroy(instantiatedNote);

        //Enable player movement
        player.GetComponent<AdvanceTestMovement>().enabled = true;
        openedNote = false;
    }

    void PointOption(TextMeshProUGUI option)
    {
        Debug.Log("pointoption " + option);
        //Change color of all options to lightGrey (118, 118, 118)
        option1.color = new Color(0.4625f, 0.4625f, 0.4625f);
        option2.color = new Color(0.4625f, 0.4625f, 0.4625f);

        //Change color of pointed option to white (255, 255, 255)
        option.color = new Color(1f, 1f, 1f);

        //Set position of pointer to pointed option
        Debug.Log(option.transform.position.y);
        pointer.transform.position =
            new Vector3(pointer.transform.position.x, option.transform.position.y, pointer.transform.position.z);

        if (option == option1) pointedOption = 1;
        else pointedOption = 1;
    }
}