using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrumpySoulSecret : MonoBehaviour
{
    [SerializeField] SkullPlatformBehavior _helpingPlatform;

    [SerializeField] GameObject _platformNotTalking;
    [SerializeField] GameObject _platformTalking;

    private void Start()
    {
        _helpingPlatform.DialogueFinished += ChangePlatforms;
    }

    void ChangePlatforms()
    {
        _platformNotTalking.SetActive(false);
        _platformTalking.SetActive(true);
        _helpingPlatform.DialogueFinished -= ChangePlatforms;
        Destroy(this);
    }
}
