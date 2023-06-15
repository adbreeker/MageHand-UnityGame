using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMana : MonoBehaviour
{
    public Text mana;
    SpellCasting spellcontroller;

    private void Start()
    {
        spellcontroller = FindObjectOfType<SpellCasting>();
    }

    void Update()
    {
        mana.text = spellcontroller.mana.ToString() + "/100";
    }
}
