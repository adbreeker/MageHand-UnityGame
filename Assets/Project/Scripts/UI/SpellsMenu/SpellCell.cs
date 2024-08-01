using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellCell : MonoBehaviour
{
    public GameObject highlight;
    public TextMeshProUGUI spellNameTMP;
    public BoxCollider hitbox;
    public void OnClick()
    {
        PlayerParams.Controllers.spellCasting.CastSpellFromName(spellNameTMP.text);
        PlayerParams.Controllers.spellsMenu.CloseMenu();
    }
}
