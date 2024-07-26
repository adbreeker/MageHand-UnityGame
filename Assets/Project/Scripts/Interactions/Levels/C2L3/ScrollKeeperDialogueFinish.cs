using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollKeeperDialogueFinish : MonoBehaviour
{
    [SerializeField] OpenWallPassage _wall;
    [SerializeField] SkullPlatformBehavior _skullPlatformBehavior;

    private void Start()
    {
        _skullPlatformBehavior.DialogueFinished += OpenWall;
    }

    void OpenWall()
    {
        _wall.Interaction();
        _skullPlatformBehavior.DialogueFinished -= OpenWall;
        Destroy(this);
    }
}
