using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpellIcon : MonoBehaviour
{
    public GameObject background;
    public void OnClick()
    {
        PlayerParams.Controllers.spellCasting.CastSpellFromName(GetComponent<TextMeshProUGUI>().text);
        PlayerParams.Controllers.spellsMenu.CloseMenu();
    }
}
