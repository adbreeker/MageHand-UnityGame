using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    [Header("Current Spell")]
    public string currentSpell = "None";
    public GameObject curretSpellObject = null;

    [Header("Spell Cast Position")]
    public GameObject hand;

    [Header("Spells Prefabs")]
    public GameObject lightPrefab;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LightSpell();
        }
        if(Input.GetKeyDown(KeyCode.Mouse0) && currentSpell == "Light")
        {
            LightBoltSpell();
        }
    }

    public void LightSpell()
    {
        if(currentSpell != "Light")
        {
            currentSpell = "Light";
            curretSpellObject = Instantiate(lightPrefab, hand.transform);
        }
        else
        {
            currentSpell = "None";
            Destroy(curretSpellObject);
        }
    }

    public void LightBoltSpell()
    {

    }
    
}
