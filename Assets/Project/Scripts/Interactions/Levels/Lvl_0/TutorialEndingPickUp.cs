using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEndingPickUp : MonoBehaviour
{
    [SerializeField] GameObject _gobletInTreasury;
    [SerializeField] LightSpellBehavior _lightInTreasury;
    [SerializeField] Transform _hud;
    [SerializeField] GameObject _flashbangEffect;

    bool _isEndingOnGoing = false;

    ProgressSaving _saveManager;
    string _nextLevel = "Level_1_Chapter_1";

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
                _isEndingOnGoing=true;
                StartCoroutine(TutorialEnding());
            }
        }
    }

    IEnumerator TutorialEnding()
    {
        _lightInTreasury.OnImpact();
        yield return new WaitForSeconds(0.15f);

        Instantiate(_flashbangEffect, _hud);
        yield return new WaitForSeconds(1f);

        _saveManager.SaveGameState(_nextLevel, PlayerParams.Controllers.plotPointsManager.plotPoints);
        _saveManager.SaveProgressToFile();

        FindObjectOfType<FadeInFadeOut>().ChangeScene(_nextLevel);
    }
}
