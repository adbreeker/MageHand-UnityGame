using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    [Header("Current Spell")]
    public string currentSpell = "None";

    [Header("Spell Cast Position")]
    public Transform hand;

    [Header("Spells Prefabs")]
    public GameObject lightPrefab;

    public void LightSpell()
    {
        if(currentSpell != "Light")
        {
            currentSpell = "Light";
            GetComponent<HandInteractions>().inHand = Instantiate(lightPrefab, hand);
        }
    }

    public void FireSpell()
    {

    }
    
}
