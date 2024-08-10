using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveWolfTests : MonoBehaviour
{
    public List<Transform> wolfPath;
    public CaveWolfController controller;

    public bool moveWolf = true;
    void Start()
    {
        if(moveWolf) { StartCoroutine(WolfMovementTest()); }
    }

    IEnumerator WolfMovementTest()
    {
        bool walk = true;
        float deley = 6.0f;
        yield return new WaitForSeconds(10.0f);

        while (true) 
        {
            if (moveWolf)
            {
                if (walk)
                {
                    deley = 10.0f;
                    controller.SetWolfMovement(wolfPath, 3.0f, 180.0f);
                }
                else
                {
                    deley = 6.0f;
                    controller.SetWolfMovement(wolfPath, true);
                }
                
                walk = !walk;
            }
            yield return new WaitForSeconds(deley);
            wolfPath.Reverse();
        }
    }
}
