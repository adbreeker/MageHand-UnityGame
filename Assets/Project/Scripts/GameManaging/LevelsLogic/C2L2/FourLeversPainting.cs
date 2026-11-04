using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourLeversPainting : MonoBehaviour
{
    [Header("Levers:")]
    [SerializeField] LeverBehavior _lever1;
    [SerializeField] LeverBehavior _lever2;
    [SerializeField] LeverBehavior _lever3;
    [SerializeField] LeverBehavior _lever4;

    [Header("Walls to interact:")]
    [SerializeField] OpenWallPassage _wall1;
    [SerializeField] OpenWallPassage _wall2;

    bool isOpen = false;

    private void Update()
    {
        if(!_lever1.leverON && !_lever2.leverON && _lever3.leverON && !_lever4.leverON)
        {
            if(!isOpen) 
            {
                isOpen = true;
                _wall1.Interaction();
                _wall2.Interaction();
            }
        }
        else
        {
            if(isOpen)
            {
                isOpen = false;
                _wall1.Interaction();
                _wall2.Interaction();
            }
        }
    }
}
