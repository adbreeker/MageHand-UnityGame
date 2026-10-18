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

    bool contestStarted = false;

    private void Awake()
    {
        GameParams.Managers.levelInfoManager.AddSecretOnLevel();
    }

    void Update()
    {
        if(contestStarted)

            if (IsStaring() && !PlayerParams.Variables.uiActive)
            {
                if (staringContestCoroutine == null)
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
        { }
    }

    bool IsStaring()
    {
        if(PlayerParams.Controllers.playerMovement.currentTile == _staringContestCube.transform)
        {
            if (Quaternion.Angle(PlayerParams.Objects.player.transform.rotation, Quaternion.identity) < 1.0f)
            {
                return true;
            }
        }
            return false;
    }

    public void StartContest()
    {
        contestStarted = true;
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
