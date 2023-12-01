using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI gestureText;
    //public TextMeshProUGUI manaText;
    public GameObject manaBox;
    public Image manaBar;
    private string ununnormalizedGestureText;

    void Update()
    {
        ShowGesture();
        ShowMana();
    }

    void ShowGesture()
    {
        if (!PlayerParams.Variables.uiActive || PlayerParams.Controllers.inventory.inventoryOpened)
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
            //manaText.text = ((int)PlayerParams.Controllers.spellCasting.mana).ToString() + "/100";
        }
        else
        {
            manaBox.SetActive(false);
            //manaText.text = "";
        }
    }
}