using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundManager;

public class FakeWallBehavior : MonoBehaviour
{
    [Header("Walls:")]
    [SerializeField] GameObject _wall1;
    [SerializeField] GameObject _wall2;


    private Material[] _wall1Materials;
    private Material[] _wall2Materials;

    Coroutine _fadingEffect = null;

    void Awake()
    {
        _wall1Materials = _wall1.GetComponent<Renderer>().materials;
        _wall2Materials = _wall2.GetComponent<Renderer>().materials;
    }

    private void OnTriggerExit(Collider other)
    {
        if(_fadingEffect == null)
        {
            _fadingEffect = StartCoroutine(FadeAlpha());
        }
    }    

    IEnumerator FadeAlpha()
    {
        //AudioSource sound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_IllusionBroken);
        //sound.Play();

        while (_wall1Materials[0].color.a > 0.1f)
        {
            Color newAlpha = _wall1Materials[0].color;
            newAlpha.a -= 0.01f;

            foreach(Material mat in _wall1Materials)
            {
                mat.color = newAlpha;
            }
            foreach (Material mat in _wall2Materials)
            {
                mat.color = newAlpha;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1f); // Wait for 1 second before increasing alpha back

        while (_wall1Materials[0].color.a < 1f)
        {
            Color newAlpha = _wall1Materials[0].color;
            newAlpha.a += 0.01f;

            foreach (Material mat in _wall1Materials)
            {
                mat.color = newAlpha;
            }
            foreach (Material mat in _wall2Materials)
            {
                mat.color = newAlpha;
            }

            yield return new WaitForFixedUpdate();
        }

        _fadingEffect = null;
        Debug.Log("Finished");
    }
}
