using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    [Header("Mana")]
    public int mana = 100;
    public int manaRegen = 2;

    [Header("Current Spell")]
    public string currentSpell = "None";
    public GameObject floatingLight;

    [Header("Spell Cast Position")]
    public Transform hand;

    [Header("Spells Prefabs")]
    public GameObject lightPrefab;

    private void Start()
    {
        StartCoroutine(RegenerateMana());
    }

    IEnumerator RegenerateMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (mana < 100)
            {
                mana += manaRegen;
            }
            mana = Mathf.Clamp(mana, 0, 100);
        }
    }

    public void LightSpell()
    {
        if(currentSpell != "Light")
        {
            currentSpell = "Light";
            GetComponent<HandInteractions>().inHand = Instantiate(lightPrefab, hand);
            mana -= 50; //zmienic potem na wartosci z ksiazki
        }
    }

    public void FireSpell()
    {

    }
    
}
