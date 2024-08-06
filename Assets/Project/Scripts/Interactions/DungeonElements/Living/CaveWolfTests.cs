using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveWolfTests : MonoBehaviour
{
    public List<Transform> wolfPath;
    public CaveWolfController controller;
    void Start()
    {
        StartCoroutine(WolfMovementTest());
    }

    IEnumerator WolfMovementTest()
    {
        yield return new WaitForSeconds(3.0f);
        controller.SetWolfMovement(wolfPath);

        while (true) 
        {
            yield return new WaitForSeconds(3.0f);
            wolfPath.Reverse();
            controller.SetWolfMovement(wolfPath);
        }
    }
}
