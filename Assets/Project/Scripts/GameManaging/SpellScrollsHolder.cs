using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScrollsHolder : MonoBehaviour
{
    //holder for spell scrolls prefabs, necessary for saves loading and dev console
    [Header("Scrolls")]
    public GameObject scrollOfLight;
    public GameObject scrollOfFire;
    public GameObject scrollOfMark;
    public GameObject scrollOfCollect;
    public GameObject scrollOfOpen;
    public GameObject scrollOfSpeak;
    public GameObject scrollOfSlow;
    public GameObject scrollOfDispel;

    public SpellScrollInfo GiveScroll(string spellName) //returning requested scroll
    {
        if (spellName == "Light")
        {
            return scrollOfLight.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }
        if (spellName == "Fire")
        {
            return scrollOfFire.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }
        if (spellName == "Mark")
        {
            return scrollOfMark.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }
        if (spellName == "Collect")
        {
            return scrollOfCollect.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }
        if (spellName == "Open")
        {
            return scrollOfOpen.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }
        if (spellName == "Speak")
        {
            return scrollOfSpeak.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }
        if (spellName == "Slow")
        {
            return scrollOfSlow.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }
        if (spellName == "Dispel")
        {
            return scrollOfDispel.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo();
        }

        return null;
    }

    public List<SpellScrollInfo> GiveAllScrolls() //returning all scrolls
    {
        List<SpellScrollInfo> allScrolls = new List<SpellScrollInfo>();
        allScrolls.Add(scrollOfLight.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo());
        allScrolls.Add(scrollOfFire.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo());
        allScrolls.Add(scrollOfMark.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo());
        allScrolls.Add(scrollOfCollect.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo());
        allScrolls.Add(scrollOfOpen.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo());
        allScrolls.Add(scrollOfSpeak.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo());
        allScrolls.Add(scrollOfSlow.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo());
        allScrolls.Add(scrollOfDispel.GetComponent<SpellScrollBehavior>().GetSpellScrollInfo());

        return allScrolls;
    }
}
