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

        while(effect.transform.localScale.y < startingScale.y*4)
        {
            yield return new WaitForFixedUpdate();
            effect.transform.localScale = Vector3.Lerp(
                effect.transform.localScale,
                new Vector3(effect.transform.localScale.x, startingScale.y*4, effect.transform.localScale.z), 0.1f);
        }

        _dialogue.SetActive(true);

        while(_dialogue.activeSelf) { yield return null; }

        while (effect.transform.localScale.y > startingScale.y)
        {
            yield return new WaitForFixedUpdate();
            effect.transform.localScale = Vector3.Lerp(
                effect.transform.localScale,
                new Vector3(effect.transform.localScale.x, startingScale.y, effect.transform.localScale.z), 0.1f);
        }

        Destroy(effect);
        PlayerParams.Controllers.playerMovement.stopMovement = false;
    }

    private void OnValidate()
    {
        if(platformActive) { GetComponent<Renderer>().material = _platformActiveMat;}
        else { GetComponent<Renderer>().material = _platformInactiveMat; }
    }
}
