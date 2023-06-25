using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScrollsHolder : MonoBehaviour
{
    [Header("Scrolls")]
    public GameObject scrollOfLight;

    public SpellScrollInfo? GiveScroll(string spellName)
    {
        if(spellName == "Light")
        {
            return scrollOfLight.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }


        return null;
    }
}
