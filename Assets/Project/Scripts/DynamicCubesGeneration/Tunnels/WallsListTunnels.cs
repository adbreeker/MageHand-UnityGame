using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsListTunnels : MonoBehaviour
{
    //wall picker for 1x1 tunnels cubes, necessary for dynamic cubes generation
    public List<GameObject> walls;

    public enum Wall
    {
        None,
        Tunnel_Wall_1,
        Tunnel_Wall_2,
        Tunnel_Wall_3,
        Tunnel_Wall_TwoSided,
        Tunnel_Wall_TwoSided_Mix,
        Tunnel_Wall_Passage,
        Tunnel_Wall_Passage_Mix,
        Tunnel_Wall_PassageLockedDoors,
        Tunnel_Wall_PassageLockedDoors_Mix,
        Tunnel_Wall_Torch,
        Tunnel_Wall_Torch_NoLight
    }
}
