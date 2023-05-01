using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsList : MonoBehaviour
{
    public List<GameObject> walls;

    public enum Wall
    {
        Wall_1,
        Wall_2,
        Wall_3,
        Wall_Bars,
        Wall_Torch,
        Wall_Window,
        Wall_TwoSided
    }
}
