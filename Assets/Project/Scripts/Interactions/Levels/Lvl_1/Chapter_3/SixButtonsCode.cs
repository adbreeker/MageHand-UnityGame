using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixButtonsCode : MonoBehaviour
{
    [Header("Passage to open")]
    [SerializeField] OpenBarsPassage _passage;

    [Header("Reset button")]
    [SerializeField] ButtonBehavior _buttonReset;

    [Header("Code buttons:")]
    [SerializeField] ButtonBehavior _button1;
    [SerializeField] ButtonBehavior _button2;
    [SerializeField] ButtonBehavior _button3;
    [SerializeField] ButtonBehavior _button4;
    [SerializeField] ButtonBehavior _button5;
    [SerializeField] ButtonBehavior _button6;

    // Update is called once per frame
    void Update()
    {
        CheckResetButton();

        if(!_passage.passageOpen)
        {
            CheckCode();
        }
    }

    void CheckResetButton()
    {
        if (_buttonReset.clickCounter > 0) 
        {
            _buttonReset.clickCounter = 0;

            _button1.clickCounter = 0;
            _button2.clickCounter = 0;
            _button3.clickCounter = 0;
            _button4.clickCounter = 0;
            _button5.clickCounter = 0;
            _button6.clickCounter = 0;

            if(_passage.passageOpen)
            {
                _passage.Interaction();
            }
        }
    }

    void CheckCode()
    {
        if(
        _button1.clickCounter == 5 && //night
        _button2.clickCounter == 3 && //mountain
        _button3.clickCounter == 3 && //deer
        _button4.clickCounter == 4 && //forest
        _button5.clickCounter == 6 && //day
        _button6.clickCounter == 1) //lake
        {
            _passage.Interaction();
        }
    }
}
