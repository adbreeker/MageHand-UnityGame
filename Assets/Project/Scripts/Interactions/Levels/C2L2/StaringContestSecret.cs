using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundManager;

public class StaringContestSecret : MonoBehaviour
{
    public float staringContestTime = 30.0f;
    [SerializeField] Transform _staringContestCube;
    [SerializeField] GameObject _dialogue;

    [Header("Secret popout")]
    public float timeToFadeOut = 2;
    public float timeOfFadingOut = 0.007f;

    [Header("After win:")]
    [SerializeField] OpenWallPassage _wallPassage;
    [SerializeField] InteractableBehavior _interactableBehavior;

    Coroutine staringContestCoroutine;

    bool dialogueStarted = false;
    AudioSource sound;

    private void Awake()
    {
        GameParams.Managers.levelInfoManager.secretsOnLevel += 1;
    }
    private void Start()
    {
        PlayerParams.Controllers.pointsManager.maxFoundSecrets += 1;
        sound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_SecretFound);
    }

    void Update()
    {
        if(!dialogueStarted) CheckDialogue();

        if (IsStaring() && dialogueStarted && !PlayerParams.Variables.uiActive)
        {
            if(staringContestCoroutine == null) 
            {
                staringContestCoroutine = StartCoroutine(StaringContest());
            }
        }
        else
        {
            if (staringContestCoroutine != null) 
            {
                StopCoroutine(staringContestCoroutine);
                staringContestCoroutine = null;
            }
        }
    }

    bool IsStaring()
    {
        if(PlayerParams.Controllers.playerMovement.currentTilePos.x == _staringContestCube.transform.position.x
            && PlayerParams.Controllers.playerMovement.currentTilePos.z == _staringContestCube.transform.position.z)
        {
            if(PlayerParams.Objects.player.transform.rotation.eulerAngles == Vector3.zero)
            {
                return true;
            }
        }
            return false;
    }

    void CheckDialogue()
    {
        if (_dialogue.activeSelf) dialogueStarted = true;
    }

    IEnumerator StaringContest()
    {
        yield return new WaitForSeconds(staringContestTime);

        sound.Play();
        PlayerParams.Controllers.pointsManager.foundSecrets += 1;
        GameParams.Managers.levelInfoManager.foundSecretsOnLevel += 1;
        string text = "Secret found!<br>" + GameParams.Managers.levelInfoManager.foundSecretsOnLevel + "/" + GameParams.Managers.levelInfoManager.secretsOnLevel;
        PlayerParams.Controllers.HUD.SpawnPopUp(text, timeToFadeOut, timeOfFadingOut, false);
        Destroy(sound, sound.clip.length);

        _wallPassage.Interaction();
        _interactableBehavior.isInteractable = true;

        Destroy(this);
    }
}
