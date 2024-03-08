using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialEndingPickUp : MonoBehaviour
{
    [SerializeField] GameObject _gobletInTreasury;
    [SerializeField] LightSpellBehavior _lightInTreasury;
    [SerializeField] Transform _hud;
    [SerializeField] GameObject _flashbangEffect;

    bool _isEndingOnGoing = false;

    ProgressSaving _saveManager;
    string _nextLevel = "Level_1_Chapter_1";
    AudioSource hitSound;
    AudioSource fallingSound;

    private void Start()
    {
        _saveManager = FindObjectOfType<ProgressSaving>();
    }

    void Update()
    {
        if(!_isEndingOnGoing)
        {
            if (PlayerParams.Controllers.handInteractions.inHand == _gobletInTreasury)
            {
                PlayerParams.Controllers.inventory.CloseInventory();
                PlayerParams.Controllers.spellbook.CloseSpellbook();
                PlayerParams.Controllers.pauseMenu.CloseMenu();
                PlayerParams.Controllers.journal.CloseJournal();

                PlayerParams.Controllers.inventory.ableToInteract = false;
                PlayerParams.Controllers.spellbook.ableToInteract = false;
                PlayerParams.Controllers.pauseMenu.ableToInteract = false;
                PlayerParams.Controllers.journal.ableToInteract = false;
                PlayerParams.Variables.uiActive = true;
                PlayerParams.Objects.hand.SetActive(false);

                hitSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Punch);
                fallingSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_BodyFall);

                _isEndingOnGoing =true;
                StartCoroutine(TutorialEnding());
            }
        }
    }

    IEnumerator TutorialEnding()
    {
        _lightInTreasury.OnImpact();
        yield return new WaitForSeconds(0.3f);
        Instantiate(_flashbangEffect, _hud);
        yield return new WaitForSeconds(1f);
        //hitSound.Play();
        //yield return new WaitForSeconds(hitSound.clip.length);

        RawImage blackoutImage = Instantiate(_flashbangEffect, _hud).GetComponent<RawImage>();
        BackgroundMusic backgroundMusic = FindObjectOfType<BackgroundMusic>();
        float startVolumeDecrease = backgroundMusic.startMusicAS.volume * 0.05f;
        float loopVolumeDecrease = backgroundMusic.loopMusicAS.volume * 0.05f;

        float alpha = 0;

        while (alpha < 1)
        {
            backgroundMusic.startMusicAS.volume -= startVolumeDecrease;
            backgroundMusic.loopMusicAS.volume -= loopVolumeDecrease;
            alpha += 0.05f;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }

        fallingSound.Play();
        yield return new WaitForSeconds(fallingSound.clip.length + 0.5f);

        _saveManager.SaveGameState(_nextLevel, PlayerParams.Controllers.pointsManager.plotPoints,
            PlayerParams.Controllers.pointsManager.maxPlotPoints,
            PlayerParams.Controllers.pointsManager.foundSecrets,
            PlayerParams.Controllers.pointsManager.maxFoundSecrets);
        _saveManager.SaveProgressToFile();

        FindObjectOfType<FadeInFadeOut>().ChangeScene(_nextLevel);
    }
}
