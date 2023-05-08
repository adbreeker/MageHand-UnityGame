using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsList2 : MonoBehaviour
{
    public List<GameObject> walls;

    public enum Wall
    {
        None,
        Big_Wall_1,
        Big_Wall_Window,
        Big_Wall_Torch,
        Duo_Wall_Passage_1,
        Duo_Wall_Passage_2,
        Duo_Wall_Bars_Passage_1,
        Duo_Wall_Bars_Passage_2,
        Big_Wall_TwoSided
    }
}
