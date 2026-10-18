using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollKeeperDialogueFinish : MonoBehaviour
{
    [SerializeField] OpenWallPassage _wall;
    [SerializeField] SkullPlatformBehavior _skullPlatformBehavior;

    private void Start()
    {
        _skullPlatformBehavior.OnDialogueFinished.AddListener(OpenWall);
    }

    void OpenWall()
    {
        _wall.Interaction();
        _skullPlatformBehavior.OnDialogueFinished.RemoveListener(OpenWall);
        Destroy(this);
    }
}
