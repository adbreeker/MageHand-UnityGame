using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Player object")]
    public GameObject player;

    [Header("Content text")]
    public string nameText;
    public string contentText;

    [Header("Options text")]
    public string option1Text;
    public string option2Text;
    public string option3Text;
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

    [Header("Parameters")]
    public float textSpeed = 0.02f;

    private Dictionary<int, TextMeshProUGUI> options = new Dictionary<int, TextMeshProUGUI>();
    private Dictionary<int, Canvas> optionsChoices = new Dictionary<int, Canvas>();
    private Dictionary<int, string> optionsTexts = new Dictionary<int, string>();

    private bool listen = false;
    private int choice = 1;

    void Start()
    {
        player.GetComponent<AdvanceTestMovement>().enabled = false;

        options.Add(1, option1);
        options.Add(2, option2);
        options.Add(3, option3);
        options.Add(4, option4);
        optionsChoices.Add(1, option1Choice);
        optionsChoices.Add(2, option2Choice);
        optionsChoices.Add(3, option3Choice);
        optionsChoices.Add(4, option4Choice);
        optionsTexts.Add(1, option1Text);
        optionsTexts.Add(2, option2Text);
        optionsTexts.Add(3, option3Text);
        optionsTexts.Add(4, option4Text);

        if (string.IsNullOrWhiteSpace(nameText))
        {
            nameTextObject.gameObject.SetActive(false);
            contentTextObject.GetComponent<RectTransform>().sizeDelta =
                new Vector2(contentTextObject.GetComponent<RectTransform>().sizeDelta.x, 200f);
        }
        else nameTextObject.text = nameText;

        pointer.transform.position =
            new Vector3(pointer.transform.position.x, options[1].transform.position.y + 4f, pointer.transform.position.z);
        options[1].color = new Color(1f, 1f, 1f);

        StartCoroutine(TypeText());
    }
    void Update()
    {
        if (listen) KeysListener();
    }

    void PointOption(TextMeshProUGUI option)
    {
        foreach (int key in options.Keys)
        {
            options[key].color = new Color(0.4625f, 0.4625f, 0.4625f); //lightGrey (118, 118, 118)
        }
        option.color = new Color(1f, 1f, 1f); //white (255, 255, 255)
        pointer.transform.position =
            new Vector3(pointer.transform.position.x, option.transform.position.y + 4f, pointer.transform.position.z);
        choice = int.Parse(option.gameObject.name);
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (choice == 1)
            {
                for (int i = 4; i > 0; i--)
                {
                    if (!string.IsNullOrWhiteSpace(options[i].text))
                    {
                        PointOption(options[i]);
                        break;
                    }
                }
            }
            else PointOption(options[choice - 1]);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (choice == 4) PointOption(option1);
            else if (string.IsNullOrWhiteSpace(options[choice + 1].text)) PointOption(option1);
            else PointOption(options[choice + 1]);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //player.GetComponent<DialogueDiary>().dialogueDiary.Add(nameText + " : " + contentText);
            //player.GetComponent<DialogueDiary>().dialogueDiary.Add("You : " + optionsTexts[choice]);
            player.GetComponent<DialogueDiary>().dialogueDiary.Add(new List<string> { nameText, contentText });
            player.GetComponent<DialogueDiary>().dialogueDiary.Add(new List<string> { "You ", optionsTexts[choice] });


            if (optionsChoices[choice] == null)
            {
                dialogueCanvas.gameObject.SetActive(false);
                player.GetComponent<AdvanceTestMovement>().enabled = true;
            }
            else
            {
                dialogueCanvas.gameObject.SetActive(false);
                optionsChoices[choice].gameObject.SetActive(true);
            }
        }
    }

    IEnumerator TypeText()
    {
        contentTextObject.text = string.Empty;
        foreach (char c in contentText.ToCharArray())
        {
            contentTextObject.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        pointer.gameObject.SetActive(true);
        yield return new WaitForSeconds(textSpeed);

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
                    yield return new WaitForSeconds(textSpeed);
                }
            }
        }
        listen = true;
    }
}
