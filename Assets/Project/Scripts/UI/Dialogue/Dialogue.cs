using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Objects")]
    public GameObject pointer; 
    public TextMeshProUGUI nameTextComponent;
    public TextMeshProUGUI contentTextComponent;
    public TextMeshProUGUI option1;
    public TextMeshProUGUI option2;
    public TextMeshProUGUI option3;
    public TextMeshProUGUI option4;
    public GameObject player;

    [Header("Text")]
    public string nameText;
    public string contentText;
    public string option1Text;
    public string option2Text;
    public string option3Text;
    public string option4Text;

    [Header("Choices")]
    public Canvas option1Choice;
    public Canvas option2Choice;
    public Canvas option3Choice;
    public Canvas option4Choice;

    [Header("Parameters")]
    public float textSpeed = 0.025f;
    public float optionsSpeed = 0.5f;
    bool showOptions = true;
    void Start()
    {
        player.GetComponent<AdvanceTestMovement>().enabled = false;
        nameTextComponent.text = nameText;
        contentTextComponent.text = string.Empty;
        StartCoroutine(TypeContent());
    }
    void Update()
    {
        KeysListener();
        if (showOptions && contentTextComponent.text == contentText)
        {
            option1.text = option1Text;
            option2.text = option2Text;
            option3.text = option3Text;
            option4.text = option4Text;
            option1.color = new Color(1f, 1f, 1f);
            StartCoroutine(TypeOptions());
            showOptions = false;
        }
    }
    IEnumerator TypeContent()
    {
        foreach (char c in contentText.ToCharArray())
        {
            contentTextComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    IEnumerator TypeOptions()
    {
        yield return new WaitForSeconds(optionsSpeed);
        option1.gameObject.SetActive(true);
        pointer.gameObject.SetActive(true);
        yield return new WaitForSeconds(optionsSpeed);
        option2.gameObject.SetActive(true);
        yield return new WaitForSeconds(optionsSpeed);
        option3.gameObject.SetActive(true);
        yield return new WaitForSeconds(optionsSpeed);
        option4.gameObject.SetActive(true);
        yield return new WaitForSeconds(optionsSpeed);
        PointOption(option2);
        yield return new WaitForSeconds(optionsSpeed);
    }
    void PointOption(TextMeshProUGUI option)
    {
        option1.color = new Color(0.4625f, 0.4625f, 0.4625f); //lightGrey (118, 118, 118)
        option2.color = new Color(0.4625f, 0.4625f, 0.4625f);
        option3.color = new Color(0.4625f, 0.4625f, 0.4625f);
        option4.color = new Color(0.4625f, 0.4625f, 0.4625f);
        option.color = new Color(1f, 1f, 1f); //white (255, 255, 255)
        pointer.transform.position = new Vector3(pointer.transform.position.x, option.transform.position.y + 4f, pointer.transform.position.z);
    }

    void KeysListener()
    {
        if (!showOptions && Input.GetKeyDown(KeyCode.W))
        {

        }

        if (!showOptions && Input.GetKeyDown(KeyCode.S))
        {

        }

        if (!showOptions && Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
}
