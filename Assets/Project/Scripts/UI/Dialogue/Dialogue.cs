using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Objects")]
    public GameObject player;
    public GameObject pointer;
    public Canvas dialogueCanvas;
    public TextMeshProUGUI nameTextComponent;
    public TextMeshProUGUI contentTextComponent;
    public TextMeshProUGUI option1;
    public TextMeshProUGUI option2;
    public TextMeshProUGUI option3;
    public TextMeshProUGUI option4;

    [Header("Text")]
    public string nameText;
    public string contentText;
    public string option1Text;
    public string option2Text;
    public string option3Text;
    public string option4Text;

    [Header("Choices")]
    public Canvas choice1;
    public Canvas choice2;
    public Canvas choice3;
    public Canvas choice4;

    [Header("Parameters")]
    public float textSpeed = 0.025f;
    public float optionsSpeed = 0.5f;

    Dictionary<int, TextMeshProUGUI> Options = new Dictionary<int, TextMeshProUGUI>();
    Dictionary<int, Canvas> Choices = new Dictionary<int, Canvas>();

    bool listen = false;
    int choice = 1;
    void Start()
    {
        player.GetComponent<AdvanceTestMovement>().enabled = false;
        nameTextComponent.text = nameText;
        contentTextComponent.text = string.Empty;
        Options.Add(1, option1);
        Options.Add(2, option2);
        Options.Add(3, option3);
        Options.Add(4, option4);
        Choices.Add(1, choice1);
        Choices.Add(2, choice2);
        Choices.Add(3, choice3);
        Choices.Add(4, choice4);
        option1.text = option1Text;
        option2.text = option2Text;
        option3.text = option3Text;
        option4.text = option4Text;
        option1.color = new Color(1f, 1f, 1f);
        StartCoroutine(TypeText());
    }
    void Update()
    {
        if(listen) KeysListener();
    }
    IEnumerator TypeText()
    {
        foreach(char c in contentText.ToCharArray())
        {
            contentTextComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(optionsSpeed);
        pointer.gameObject.SetActive(true);

        foreach (int key in Options.Keys)
        {
            if(string.IsNullOrWhiteSpace(Options[key].text)) break;
            Options[key].gameObject.SetActive(true);
            yield return new WaitForSeconds(optionsSpeed);
        }

        yield return new WaitForSeconds(textSpeed);
        listen = true;
    }
    void PointOption(TextMeshProUGUI option)
    {
        option1.color = new Color(0.4625f, 0.4625f, 0.4625f); //lightGrey (118, 118, 118)
        option2.color = new Color(0.4625f, 0.4625f, 0.4625f);
        option3.color = new Color(0.4625f, 0.4625f, 0.4625f);
        option4.color = new Color(0.4625f, 0.4625f, 0.4625f);
        option.color = new Color(1f, 1f, 1f); //white (255, 255, 255)
        pointer.transform.position = new Vector3(pointer.transform.position.x, option.transform.position.y + 4f, pointer.transform.position.z);
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
                    if (!string.IsNullOrWhiteSpace(Options[i].text))
                    {
                        PointOption(Options[i]);
                        break;
                    }
                }
            }
            else PointOption(Options[choice-1]);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (choice == 4) PointOption(option1);
            else if (string.IsNullOrWhiteSpace(Options[choice+1].text)) PointOption(option1);
            else PointOption(Options[choice+1]);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Choices[choice] == null)
            {
                dialogueCanvas.gameObject.SetActive(false);
                player.GetComponent<AdvanceTestMovement>().enabled = true;
            }
            else
            {
                dialogueCanvas.gameObject.SetActive(false);
                Choices[choice].gameObject.SetActive(true);
            }
        }
    }
}
