using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScrollsHolder : MonoBehaviour
{
    public List<GameObject> spellScrolls;

    public GameObject GiveSpellScroll(string spellName)
    {
        foreach(GameObject spellScroll in spellScrolls)
        {
            if(spellScroll.GetComponent<SpellScrollBehavior>().spellName == spellName)
            {
                return spellScroll;
            }
        }
        return null;
    }
}
