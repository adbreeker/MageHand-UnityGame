using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMana : MonoBehaviour //displaying mana resource on debug canvas
{
    public Text mana;

    void Update()
    {
        mana.text = ((int)PlayerParams.Controllers.spellCasting.mana).ToString() + "/100";
    }
}
