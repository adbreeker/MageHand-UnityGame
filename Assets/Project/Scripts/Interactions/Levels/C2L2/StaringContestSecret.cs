using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaringContestSecret : MonoBehaviour
{
    public float staringContestTime = 30.0f;
    [SerializeField] Transform _staringContestCube;
    [SerializeField] GameObject _dialogue;

    [Header("After win:")]
    [SerializeField] OpenWallPassage _wallPassage;
    [SerializeField] InteractableBehavior _interactableBehavior;

    Coroutine staringContestCoroutine;

    bool dialogueStarted = false;

    private void Awake()
    {
        GameParams.Managers.levelInfoManager.AddSecretOnLevel();
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
        if(PlayerParams.Controllers.playerMovement.currentTile == _staringContestCube.transform)
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

        GameParams.Managers.levelInfoManager.SecretFoundPopUp();

        _wallPassage.Interaction();
        _interactableBehavior.isInteractable = true;

        Destroy(this);
    }
}
