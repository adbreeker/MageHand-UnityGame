using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsList3 : MonoBehaviour
{
    public List<GameObject> walls;

    public enum Wall
    {
        None,
        Tri_Wall_1,
        Tri_Wall_2,
        Tri_Wall_3,
        Tri_Wall_Window,
        Tri_Wall_Torch,
        Tri_Wall_Passage_1,
        Tri_Wall_Passage_2,
        Tri_Wall_Passage_3,
        Tri_Wall_Bars_Passage_1,
        Tri_Wall_Bars_Passage_2,
        Tri_Wall_Bars_Passage_3,
        Tri_Wall_TwoSided
    }
}
