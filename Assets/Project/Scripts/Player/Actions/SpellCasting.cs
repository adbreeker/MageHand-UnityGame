using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    [Header("Mana")]
    public float mana = 100.0f;
    public float manaRegen = 25.0f;

    [Header("Current Spell")]
    public string currentSpell = "None";
    public GameObject floatingLight;

    [Header("Spell Cast Position")]
    public Transform hand;

    [Header("Spells Prefabs")]
    public GameObject lightPrefab;

    private void Update()
    {
        RegenerateMana();
    }

    void RegenerateMana()
    {
        mana += manaRegen * Time.deltaTime;
        mana = Mathf.Clamp(mana, 0.0f, 100.0f);
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
