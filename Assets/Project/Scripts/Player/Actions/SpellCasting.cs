using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    [Header("Current Spell")]
    public string currentSpell = "None";

    [Header("Spell Cast Position")]
    public GameObject hand;

    [Header("Spells Prefabs")]
    public GameObject lightPrefab;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            if(gameObject.GetComponent<HandInteractions>().inHand == null)
            {
                LightSpell();
            }
        }
    }

    public void LightSpell()
    {
        if(currentSpell != "Light")
        {
            currentSpell = "Light";
            this.GetComponent<HandInteractions>().inHand = Instantiate(lightPrefab, hand.transform);
        }
        else
        {
            currentSpell = "None";
            Destroy(this.GetComponent<HandInteractions>().inHand);
        }
    }

    public void FireSpell()
    {

    }
    
}
