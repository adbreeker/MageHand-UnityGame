using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeTunnelWallBehavior : MonoBehaviour
{
    [Header("Walls:")]
    [SerializeField] GameObject _wall1;
    [SerializeField] GameObject _wall2;

    [Header("Fake materials:")]
    [SerializeField] Material _fakeWallMaterial;
    [SerializeField] Material _fakeClayWallMaterial;

    private Renderer _wall1Renderer;
    private Renderer _wall2Renderer;

    Coroutine _fadingEffect = null;

    void Awake()
    {
        _wall1Renderer = _wall1.GetComponent<Renderer>();
        _wall2Renderer = _wall2.GetComponent<Renderer>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_fadingEffect == null)
        {
            _fadingEffect = StartCoroutine(FadeAlpha());
        }
    }

    IEnumerator FadeAlpha()
    {
        //AudioSource sound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_IllusionBroken);
        //sound.Play();

        Material[] prevWall1Materials = _wall1Renderer.materials;
        Material[] prevWall2Materials = _wall2Renderer.materials;

        _wall1Renderer.materials = new Material[] { _fakeClayWallMaterial, _fakeWallMaterial};
        _wall2Renderer.materials = new Material[] { _fakeClayWallMaterial, _fakeWallMaterial };

        Material[] wall1Materials = _wall1Renderer.materials;
        Material[] wall2Materials = _wall2Renderer.materials;

        while (wall1Materials[0].color.a > 0.1f)
        {
            Color newAlpha = wall1Materials[0].color;
            newAlpha.a -= 0.01f;

            wall1Materials[0].color = newAlpha;
            wall1Materials[1].color = newAlpha;
            wall2Materials[0].color = newAlpha;
            wall2Materials[1].color = newAlpha;

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1f); // Wait for 1 second before increasing alpha back

        while (wall1Materials[0].color.a < 1f)
        {
            Color newAlpha = wall1Materials[0].color;
            newAlpha.a += 0.01f;

            wall1Materials[0].color = newAlpha;
            wall1Materials[1].color = newAlpha;
            wall2Materials[0].color = newAlpha;
            wall2Materials[1].color = newAlpha;

            yield return new WaitForFixedUpdate();
        }

        _fadingEffect = null;

        _wall1Renderer.materials = prevWall1Materials;
        _wall2Renderer.materials = prevWall2Materials;
    }
}
