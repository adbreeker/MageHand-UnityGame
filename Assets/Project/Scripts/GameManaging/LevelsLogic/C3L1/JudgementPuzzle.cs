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

    List<int> _judgementsUsed = new List<int>();
    bool goodJudgementUsed = false;

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
        if(!_judgementsUsed.Contains(optionIndex))
        {
            _judgementsUsed.Add(optionIndex);
            PlayerParams.Controllers.pointsManager.plotPoints -= 1;

            _skull.SetActive(false);
        }

        PlayerParams.Controllers.playerMovement.TeleportTo(_failTpDestination, 0f,  null);
    }

    public void JudgePlatformReactivation()
    {
        if (!goodJudgementUsed) { _judgePlatform.SetPlatformActive(true); }
    }
}