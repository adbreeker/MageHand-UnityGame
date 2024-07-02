using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameParams
{
    public static class Holders
    {
        public static MaterialsAndEffectsHolder materialsAndEffectsHolder;
        public static ItemHolder itemHolder;
        public static SpellScrollsHolder spellScrollsHolder;
    }

    public static class Managers
    {
        public static GameManager gameManager;
        public static GameSettings gameSettings;
        public static ProgressSaving saveManager;
        public static SoundManager soundManager;
        public static FadeInFadeOut fadeInOutManager;
        public static LevelInfoDisplay levelInfoManager;
    }

    public static class Variables
    {
        public static float currentTimeScale = 1.0f;
    }
}

public class GameManager : MonoBehaviour
{
    [Header("Holders:")]
    public MaterialsAndEffectsHolder materialsAndEffectsHolder;
    public ItemHolder itemHolder;
    public SpellScrollsHolder spellScrollsHolder;

    [Header("Managers:")]
    public GameSettings gameSettings;
    public ProgressSaving saveManager;
    public SoundManager soundManager;
    public FadeInFadeOut fadeInOutManager;
    public LevelInfoDisplay levelInfoManager;

    private void Awake()
    {
        GameParams.Holders.materialsAndEffectsHolder = materialsAndEffectsHolder;
        GameParams.Holders.itemHolder = itemHolder;
        GameParams.Holders.spellScrollsHolder = spellScrollsHolder;

        GameParams.Managers.gameManager = this;
        GameParams.Managers.gameSettings = gameSettings;
        GameParams.Managers.saveManager = saveManager;
        GameParams.Managers.soundManager = soundManager;
        GameParams.Managers.fadeInOutManager = fadeInOutManager;
        GameParams.Managers.levelInfoManager = levelInfoManager;

        GameParams.Variables.currentTimeScale = 1.0f;
    }
}
