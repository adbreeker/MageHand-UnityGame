using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowGesture : MonoBehaviour
{
    private TextMeshProUGUI gestureText;
    private string ununnormalizedText;

    public void Awake()
    {
        gestureText = gameObject.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        ununnormalizedText = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("Hand").GetComponent<MoveHandPoints>().gesture;

        if (ununnormalizedText == "Pointing_Up") gestureText.text = "Click";
        else if (ununnormalizedText == "Closed_Fist") gestureText.text = "Grab";
        else if(ununnormalizedText == "Thumb_Up") gestureText.text = "Throw";
        else if(ununnormalizedText == "Victory") gestureText.text = "Cast";
        else if(ununnormalizedText == "Thumb_Down") gestureText.text = "Equip/Reactivation";
        else if(ununnormalizedText == "ILoveYou") gestureText.text = "Drink";
        else gestureText.text = "None";




    }
}
