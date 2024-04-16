using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundManager;

public class FakeWallBehavior : MonoBehaviour
{
    [Header("Walls:")]
    [SerializeField] GameObject _wall1;
    [SerializeField] GameObject _wall2;


    private Material _wall1Material;
    private Material _wall2Material;

    Coroutine _fadingEffect = null;

    void Awake()
    {
        _wall1Material = _wall1.GetComponent<Renderer>().material;
        _wall2Material = _wall2.GetComponent<Renderer>().material;
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

        while (_wall1Material.color.a > 0.1f)
        {
            Color newAlpha = _wall1Material.color;
            newAlpha.a -= 0.01f;

            _wall1Material.color = newAlpha;
            _wall2Material.color = newAlpha;

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1f); // Wait for 1 second before increasing alpha back

        while (_wall1Material.color.a < 1f)
        {
            Color newAlpha = _wall1Material.color;
            newAlpha.a += 0.01f;

            _wall1Material.color = newAlpha;
            _wall2Material.color = newAlpha;

            yield return new WaitForFixedUpdate();
        }

        _fadingEffect = null;
        Debug.Log("Finished");
    }
}
