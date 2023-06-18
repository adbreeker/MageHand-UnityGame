using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScrollsHolder : MonoBehaviour
{
    [Header("Scrolls")]
    public GameObject scrollOfLight;

    public GameObject GiveScroll(string spellName)
    {
        if(spellName == "Light")
        {
            return Instantiate(scrollOfLight, new Vector3(0, 10, 0), Quaternion.identity);
        }


        return null;
    }
}
