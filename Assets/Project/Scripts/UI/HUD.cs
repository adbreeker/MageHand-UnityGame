using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FMODUnity;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI gestureText;
    public GameObject manaBox;
    public Image manaBar;
    public TextMeshProUGUI manaText;
    public GameObject popUpContainer;
    public GameObject popUpPrefab;

    private GameObject instantiatedPopUp;
    private string ununnormalizedGestureText;

    void Update()
    {
        ShowGesture();
        ShowMana();
    }

    void ShowGesture()
    {
        if (!PlayerParams.Variables.uiActive || PlayerParams.Controllers.inventory.inventoryOpened || PlayerParams.Controllers.spellsMenu.menuOpened)
        {
            ununnormalizedGestureText = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("Hand").GetComponent<MoveHandPoints>().gesture;
            if (ununnormalizedGestureText == "Pointing_Up") gestureText.text = "Interact";
            else if (ununnormalizedGestureText == "Closed_Fist") gestureText.text = "Grab";
            else if (ununnormalizedGestureText == "Thumb_Up") gestureText.text = "Throw";
            else if (ununnormalizedGestureText == "Victory") gestureText.text = "Cast";
            else if (ununnormalizedGestureText == "Thumb_Down") gestureText.text = "Equip";
            else if (ununnormalizedGestureText == "ILoveYou") gestureText.text = "Drink";
            else gestureText.text = "";
        }
        else
        {
            gestureText.text = "";
        }
    }

    void ShowMana()
    {
        if (!PlayerParams.Variables.uiActive && PlayerParams.Controllers.spellbook.spells.Count > 0)
        {
            manaBox.SetActive(true);
            manaBar.fillAmount = PlayerParams.Controllers.spellCasting.mana / 100;
            manaText.text = (int)PlayerParams.Controllers.spellCasting.mana + "/100";
        }
        else
        {
            manaBox.SetActive(false);
        }
    }


    public void SpawnPopUp(string content, float timeToFadeOut, float timeOfFadingOut, bool playSound = true)
    {
        instantiatedPopUp = Instantiate(popUpPrefab, new Vector3(0, 0, 0), Quaternion.identity, popUpContainer.transform);

        TextMeshProUGUI contentObject = instantiatedPopUp.transform.Find("Content").GetComponent<TextMeshProUGUI>();

        contentObject.text = content;

        if (playSound) RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiPopUp);

        StartCoroutine(FadeOutPopUp(instantiatedPopUp, timeToFadeOut, timeOfFadingOut, playSound));
    }


    IEnumerator FadeOutPopUp(GameObject popUp, float timeToFadeOut, float timeOfFadingOut, bool playSound)
    {
        yield return new WaitForSeconds(timeToFadeOut);
        while (popUp.GetComponent<CanvasGroup>().alpha > 0)
        {
            popUp.GetComponent<CanvasGroup>().alpha -= timeOfFadingOut;
            yield return new WaitForFixedUpdate();
        }
        Destroy(popUp);
    }
}