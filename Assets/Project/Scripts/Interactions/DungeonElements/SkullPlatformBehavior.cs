using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullPlatformBehavior : MonoBehaviour
{
    public bool platformActive = true;

    [Header("Linked dialogue")]
    [SerializeField] GameObject _dialogue;

    [Header("Materials:")]
    [SerializeField] Material _platformActiveMat;
    [SerializeField] Material _platformInactiveMat;

    [Header("Platform effect")]
    [SerializeField] GameObject _platformEffectPrefab;

    //[Header("Platform outcomes:")]
    public event Action DialogueFinished;

    public void UsePlatform()
    {
        platformActive = false;
        StartCoroutine(PlatformEffect());
        PlayerParams.Controllers.playerMovement.stopMovement = true;
    }

    IEnumerator PlatformEffect()
    {
        GameObject effect = Instantiate(_platformEffectPrefab, transform);
        GetComponent<Renderer>().material = _platformInactiveMat;

        Vector3 startingScale = effect.transform.localScale;

        while(effect.transform.localScale.y != startingScale.y*6.0f)
        {
            yield return new WaitForFixedUpdate();
            effect.transform.localScale = Vector3.MoveTowards(
                effect.transform.localScale,
                new Vector3(effect.transform.localScale.x, startingScale.y*6.0f, effect.transform.localScale.z), 0.05f);
        }

        yield return new WaitForSeconds(1);

        _dialogue.SetActive(true);

        while(_dialogue.activeSelf) { yield return null; }
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
        if(platformActive) { GetComponent<Renderer>().material = _platformActiveMat;}
        else { GetComponent<Renderer>().material = _platformInactiveMat; }
    }
}
