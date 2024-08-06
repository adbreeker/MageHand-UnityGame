using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchmanDialogueFinish : MonoBehaviour
{
    [SerializeField] OpenBarsPassage _barsPassage;
    [SerializeField] SkullPlatformBehavior _skullPlatformBehavior;

    private void Start()
    {
        _skullPlatformBehavior.DialogueFinished += OpenBars;
    }

    void OpenBars()
    {
        _barsPassage.Interaction();
        _skullPlatformBehavior.DialogueFinished -= OpenBars;
        Destroy(this);
    }
}
