using System.Collections.Generic;
using UnityEngine;

public class JudgementPuzzle : MonoBehaviour
{
    [Header("Win:")]
    [SerializeField] OpenWallPassage _treasuryWall;

    [Header("Fail:")]
    [SerializeField] Vector3 _failTpDestination;
    [SerializeField] GameObject _skull;

    [Header("Judge skull platform")]
    [SerializeField] SkullPlatformBehavior _judgePlatform;

    List<int> judgementsUsed = new List<int>();

    private void Start()
    {
        PlayerParams.Controllers.pointsManager.maxPlotPoints += 1;
        PlayerParams.Controllers.pointsManager.minPlotPoints += -2;
    }

    public void GoodJudgement()
    {
        PlayerParams.Controllers.pointsManager.plotPoints += 1;
        _treasuryWall.Interaction();

    }

    public void BadJudgement(int optionIndex)
    {
        if(!judgementsUsed.Contains(optionIndex))
        {
            judgementsUsed.Add(optionIndex);
            PlayerParams.Controllers.pointsManager.plotPoints -= 1;

            _skull.SetActive(false);
        }

        PlayerParams.Controllers.playerMovement.TeleportTo(_failTpDestination, 0f,  null);
    }
}