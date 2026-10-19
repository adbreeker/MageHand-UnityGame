using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkullPlatformBehavior : MonoBehaviour
{
    public bool platformActive = true;

    [Header("Linked dialogue")]
    [SerializeField] OpenDialogue _dialogue;

    [Header("Materials:")]
    [SerializeField] Renderer _renderer;
    [SerializeField] Material _platformActiveMat;
    [SerializeField] Material _platformInactiveMat;

    [Header("Platform effect")]
    [SerializeField] GameObject _platformEffectPrefab;
    GameObject _platformEffect;
    Vector3 _effectStartingScale;

    [Header("Platform outcomes:")]
    public UnityEvent OnDialogueFinished;

    public void SetPlatformActive(bool active)
    {
        if(active) 
        {
            platformActive = true;
            _renderer.material = _platformActiveMat;
            _dialogue.gameObject.SetActive(true);
        }
        else
        {
            platformActive = false;
            _renderer.material = _platformInactiveMat;
        }
    }

    public void UsePlatform()
    {
        SetPlatformActive(false);
        StartCoroutine(PlatformEffectStart());
        PlayerParams.Controllers.playerMovement.stopMovement = true;
        _dialogue.OnDialogueFinished.AddListener(DisablePlatform);
    }

    IEnumerator PlatformEffectStart()
    {
        _platformEffect = Instantiate(_platformEffectPrefab, transform);

        _effectStartingScale = _platformEffect.transform.localScale;

        while(_platformEffect.transform.localScale.y != _effectStartingScale.y*6.0f)
        {
            yield return new WaitForFixedUpdate();
            _platformEffect.transform.localScale = Vector3.MoveTowards(
                _platformEffect.transform.localScale,
                new Vector3(_platformEffect.transform.localScale.x, _effectStartingScale.y*6.0f, _platformEffect.transform.localScale.z), 0.05f);
        }

        yield return new WaitForSeconds(1);

        _dialogue.allowToActivate = true;
    }

    IEnumerator PlatformEffectEnd()
    {
        PlayerParams.Controllers.playerMovement.stopMovement = false;
        PlayerParams.Controllers.spellCasting.currentSpell = "None";
        OnDialogueFinished?.Invoke();

        while ( _platformEffect.transform.localScale.y > _effectStartingScale.y)
        {
            yield return new WaitForFixedUpdate();
            _platformEffect.transform.localScale = Vector3.MoveTowards(
                _platformEffect.transform.localScale,
                new Vector3(_platformEffect.transform.localScale.x, _effectStartingScale.y, _platformEffect.transform.localScale.z), 0.05f);
        }

        Destroy(_platformEffect);
    }

    public void DisablePlatform()
    {
        _dialogue.OnDialogueFinished.RemoveListener(DisablePlatform);
        StartCoroutine(PlatformEffectEnd());
    }

    private void OnValidate()
    {
        if(platformActive) { _renderer.material = _platformActiveMat;}
        else { _renderer.material = _platformInactiveMat; }
    }
}
