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

    [Header("Code tablets:")]
    [SerializeField] TextTabletBehavior _TextTablet1;
    [SerializeField] TextTabletBehavior _TextTablet2;
    [SerializeField] TextTabletBehavior _TextTablet3;
    [SerializeField] TextTabletBehavior _TextTablet4;
    [SerializeField] TextTabletBehavior _TextTablet5;
    [SerializeField] TextTabletBehavior _TextTablet6;

    // Update is called once per frame
    void Update()
    {
        CheckResetButton();

        if(!_passage.passageOpen)
        {
            CheckCode();
        }

        UpdateTextTablets();
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

    void UpdateTextTablets()
    {
        _TextTablet1.tabletText = _button1.clickCounter.ToString();
        _TextTablet2.tabletText = _button2.clickCounter.ToString();
        _TextTablet3.tabletText = _button3.clickCounter.ToString();
        _TextTablet4.tabletText = _button4.clickCounter.ToString();
        _TextTablet5.tabletText = _button5.clickCounter.ToString();
        _TextTablet6.tabletText = _button6.clickCounter.ToString();
    }
}
