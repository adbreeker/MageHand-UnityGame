using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCrystalBallsPuzzle : MonoBehaviour
{
    [Header("Crystal balls detectors:")]
    [SerializeField] List<ItemDetecting> _ballsDetectors;

    [Header("Wall to open:")]
    [SerializeField] OpenWallPassage _wall;

    private void Update()
    {
        CheckDetectors();
    }

    void CheckDetectors()
    {
        bool allBalls = true;
        foreach(ItemDetecting detector in _ballsDetectors) 
        {
            if(!detector.isItemInBoundaries)
            {
                allBalls = false; 
                break;
            }
        }

        if(allBalls && !_wall.passageOpen) { _wall.Interaction(); }
        if(!allBalls && _wall.passageOpen) { _wall.Interaction(); }
    }
}
