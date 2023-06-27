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
        Duo_wall_1,
        Duo_wall_2,
        Duo_wall_3,
        Big_Wall_Window,
        Big_Wall_Torch,
        Big_Wall_Torch_NoLight,
        Duo_Wall_Passage_1,
        Duo_Wall_Passage_2,
        Duo_Wall_Bars_Passage_1,
        Duo_Wall_Bars_Passage_2,
        Big_Wall_TwoSided,
        Duo_Wall_TwoSided,
        Duo_Wall_Hole_1,
        Duo_Wall_Hole_2,
        Duo_Wall_LockedDoors_1,
        Duo_Wall_LockedDoors_2
    }
}
