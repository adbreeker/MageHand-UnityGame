using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiral3ButtonsSecret : MonoBehaviour
{
    [Header("Buttons:")]
    [SerializeField] ButtonBehavior _button1;
    [SerializeField] ButtonBehavior _button2;
    [SerializeField] ButtonBehavior _button3;

    [Header("Time for all 3:")]
    [SerializeField] int _time;

    [Header("Secret")]
    [SerializeField] Vector3 _tpPos;
    [SerializeField] OpenWallPassage _wall;

    Coroutine _button1Check;
    Coroutine _button2Check;
    Coroutine _button3Check;

    private void Update()
    {
        if(_button1.clickCounter > 0)
        {
            if(_button1.clickCounter == 1 && _button1Check == null)
            {
                _button1Check = StartCoroutine(ClickTimer1());
            }
            if(_button1.clickCounter > 1 && _button1Check != null)
            {
                _button1.clickCounter = 1;
                StopCoroutine(_button1Check);
                _button1Check = StartCoroutine(ClickTimer1());
            }
        }

        if (_button2.clickCounter > 0)
        {
            if (_button2.clickCounter == 1 && _button2Check == null)
            {
                _button2Check = StartCoroutine(ClickTimer2());
            }
            if (_button2.clickCounter > 1 && _button2Check != null)
            {
                _button2.clickCounter = 1;
                StopCoroutine(_button2Check);
                _button2Check = StartCoroutine(ClickTimer2());
            }
        }

        if (_button3.clickCounter > 0)
        {
            if (_button3.clickCounter == 1 && _button3Check == null)
            {
                _button3Check = StartCoroutine(ClickTimer3());
            }
            if (_button3.clickCounter > 1 && _button3Check != null)
            {
                _button3.clickCounter = 1;
                StopCoroutine(_button3Check);
                _button3Check = StartCoroutine(ClickTimer3());
            }
        }

        if(_button1.clickCounter > 0 && _button2.clickCounter > 0 && _button3.clickCounter > 0)
        {
            PlayerParams.Controllers.playerMovement.TeleportTo(_tpPos, null);
            _wall.Interaction();
            Destroy(this);
        }
    }

    IEnumerator ClickTimer1()
    {
        yield return new WaitForSeconds(_time);
        _button1.clickCounter = 0;
        _button1Check = null;
    }

    IEnumerator ClickTimer2()
    {
        yield return new WaitForSeconds(_time);
        _button2.clickCounter = 0;
        _button2Check = null;
    }

    IEnumerator ClickTimer3()
    {
        yield return new WaitForSeconds(_time);
        _button3.clickCounter = 0;
        _button3Check = null;
    }
}
