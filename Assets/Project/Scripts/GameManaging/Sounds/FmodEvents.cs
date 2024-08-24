using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference MUSIC_Dungeon1 { get; private set; }
    [field: SerializeField] public EventReference MUSIC_Dungeon2 { get; private set; }
    [field: SerializeField] public EventReference MUSIC_Dungeon3 { get; private set; }
    [field: SerializeField] public EventReference MUSIC_Dungeon4 { get; private set; }
    [field: SerializeField] public EventReference MUSIC_Dungeon5 { get; private set; }
    [field: SerializeField] public EventReference MUSIC_MainTheme { get; private set; }
    [field: SerializeField] public EventReference MUSIC_LoadingAmbient { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference UI_ChangeOption { get; private set; }
    [field: SerializeField] public EventReference UI_SelectOption { get; private set; }
    [field: SerializeField] public EventReference UI_Close { get; private set; }
    [field: SerializeField] public EventReference UI_Open { get; private set; }
    [field: SerializeField] public EventReference UI_PopUp { get; private set; }

    [field: Header("SFX Voices")]
    [field: SerializeField] public EventReference VOICE_Guide { get; private set; }
    [field: SerializeField] public EventReference VOICE_Alandos { get; private set; }

    [field: Header("SFX Reading")]
    [field: SerializeField] public EventReference READ_Light { get; private set; }
    [field: SerializeField] public EventReference READ_Mark { get; private set; }
    [field: SerializeField] public EventReference READ_Fire { get; private set; }
    [field: SerializeField] public EventReference READ_Speak { get; private set; }

    [field: Header("SFX Spells")]
    [field: SerializeField] public EventReference SFX_CastingSpell { get; private set; }
    [field: SerializeField] public EventReference SFX_CastingSpellFailed { get; private set; }
    [field: SerializeField] public EventReference SFX_CastingSpellFinished { get; private set; }
    [field: SerializeField] public EventReference SFX_OpenSpell { get; private set; }
    [field: SerializeField] public EventReference SFX_SpellLightRemaining { get; private set; }
    [field: SerializeField] public EventReference SFX_SpellLightBurst { get; private set; }
    [field: SerializeField] public EventReference SFX_MarkSpellActivation { get; private set; }
    [field: SerializeField] public EventReference SFX_FireSpellRemaining { get; private set; }
    [field: SerializeField] public EventReference SFX_FireSpellBurst { get; private set; }

    [field: Header("SFX Environment")]
    [field: SerializeField] public EventReference SFX_StepStone { get; private set; }
    [field: SerializeField] public EventReference SFX_OpenChest { get; private set; }
    [field: SerializeField] public EventReference SFX_CloseChest { get; private set; }
    [field: SerializeField] public EventReference SFX_Button { get; private set; }
    [field: SerializeField] public EventReference SFX_LeverToUp { get; private set; }
    [field: SerializeField] public EventReference SFX_LeverToDown { get; private set; }
    [field: SerializeField] public EventReference SFX_PickUpItem { get; private set; }
    [field: SerializeField] public EventReference SFX_PutToInventory { get; private set; }
    [field: SerializeField] public EventReference SFX_Drink { get; private set; }
    [field: SerializeField] public EventReference SFX_UnlockOpenDoor { get; private set; }
    [field: SerializeField] public EventReference SFX_Collision { get; private set; }
    [field: SerializeField] public EventReference SFX_MovingWall { get; private set; }
    [field: SerializeField] public EventReference SFX_MovingMetalGate { get; private set; }
    [field: SerializeField] public EventReference SFX_Earthquake { get; private set; }
    [field: SerializeField] public EventReference SFX_BodyFall { get; private set; }
    [field: SerializeField] public EventReference SFX_Punch { get; private set; }
    [field: SerializeField] public EventReference SFX_IllusionBroken { get; private set; }
    [field: SerializeField] public EventReference SFX_MagicalTeleportation { get; private set; }
    [field: SerializeField] public EventReference SFX_SecretFound { get; private set; }
    [field: SerializeField] public EventReference SFX_LevelInfo { get; private set; }
}
