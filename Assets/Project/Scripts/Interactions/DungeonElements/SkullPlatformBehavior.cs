using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //[Header("Platform outcomes:")]
    public event Action DialogueFinished;

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
        StartCoroutine(PlatformEffect());
        PlayerParams.Controllers.playerMovement.stopMovement = true;
    }

    IEnumerator PlatformEffect()
    {
        GameObject effect = Instantiate(_platformEffectPrefab, transform);

        Vector3 startingScale = effect.transform.localScale;

        while(effect.transform.localScale.y != startingScale.y*6.0f)
        {
            yield return new WaitForFixedUpdate();
            effect.transform.localScale = Vector3.MoveTowards(
                effect.transform.localScale,
                new Vector3(effect.transform.localScale.x, startingScale.y*6.0f, effect.transform.localScale.z), 0.05f);
        }

        yield return new WaitForSeconds(1);

        _dialogue.allowToActivate = true;

        while(_dialogue.gameObject.activeSelf) { yield return null; }
        PlayerParams.Controllers.playerMovement.stopMovement = false;
        PlayerParams.Controllers.spellCasting.currentSpell = "None";
        DialogueFinished?.Invoke();

        while (effect.transform.localScale.y > startingScale.y)
        {
            yield return new WaitForFixedUpdate();
            effect.transform.localScale = Vector3.MoveTowards(
                effect.transform.localScale,
                new Vector3(effect.transform.localScale.x, startingScale.y, effect.transform.localScale.z), 0.05f);
        }

        Destroy(effect);
    }

    private void OnValidate()
    {
        if(platformActive) { _renderer.material = _platformActiveMat;}
        else { _renderer.material = _platformInactiveMat; }
    }
}
