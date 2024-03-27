using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixButtonsCode : MonoBehaviour
{
    [Header("Passage to open")]
    [SerializeField] OpenBarsPassage _passage;

    [Header("Reset button")]
    [SerializeField] ButtonBehavior _buttonReset;

    [Header("Code buttons add:")]
    [SerializeField] ButtonBehavior _button1;
    [SerializeField] ButtonBehavior _button2;
    [SerializeField] ButtonBehavior _button3;
    [SerializeField] ButtonBehavior _button4;
    [SerializeField] ButtonBehavior _button5;
    [SerializeField] ButtonBehavior _button6;

    [Header("Code buttons subtract:")]
    [SerializeField] ButtonBehavior _button1s;
    [SerializeField] ButtonBehavior _button2s;
    [SerializeField] ButtonBehavior _button3s;
    [SerializeField] ButtonBehavior _button4s;
    [SerializeField] ButtonBehavior _button5s;
    [SerializeField] ButtonBehavior _button6s;

    int code1 = 0;
    int code2 = 0;
    int code3 = 0;
    int code4 = 0;
    int code5 = 0;
    int code6 = 0;

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

        code1 = ((_button1.clickCounter - _button1s.clickCounter) % 10 + 10) % 10;
        code2 = ((_button2.clickCounter - _button2s.clickCounter) % 10 + 10) % 10;
        code3 = ((_button3.clickCounter - _button3s.clickCounter) % 10 + 10) % 10;
        code4 = ((_button4.clickCounter - _button4s.clickCounter) % 10 + 10) % 10;
        code5 = ((_button5.clickCounter - _button5s.clickCounter) % 10 + 10) % 10;
        code6 = ((_button6.clickCounter - _button6s.clickCounter) % 10 + 10) % 10;

        if (!_passage.passageOpen)
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

            _button1s.clickCounter = 0;
            _button2s.clickCounter = 0;
            _button3s.clickCounter = 0;
            _button4s.clickCounter = 0;
            _button5s.clickCounter = 0;
            _button6s.clickCounter = 0;

            if (_passage.passageOpen)
            {
                _passage.Interaction();
            }
        }
    }

    void CheckCode()
    {
        if(
        code1 == 5 && //night
        code2 == 3 && //mountain
        code3 == 3 && //deer
        code4 == 4 && //forest
        code5 == 6 && //day
        code6 == 1) //lake
        {
            _passage.Interaction();
        }
    }

    void UpdateTextTablets()
    {
        _TextTablet1.tabletText = code1.ToString();
        _TextTablet2.tabletText = code2.ToString();
        _TextTablet3.tabletText = code3.ToString();
        _TextTablet4.tabletText = code4.ToString();
        _TextTablet5.tabletText = code5.ToString();
        _TextTablet6.tabletText = code6.ToString();
    }
}
