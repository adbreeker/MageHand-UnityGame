using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsList1 : MonoBehaviour
{
    //wall picker for 1x1 cubes, necessary for dynamic cubes generation
    public List<GameObject> walls;

    public enum Wall
    {
        None,
        Wall_1,
        Wall_2,
        Wall_3,
        Wall_Bars_Passage,
        Wall_Passage,
        Wall_Torch,
        Wall_Torch_NoLight,
        Wall_Window,
        Wall_TwoSided,
        Wall_LockedDoorsPassage
    }
}
