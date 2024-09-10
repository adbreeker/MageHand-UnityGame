using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeWallBehavior : MonoBehaviour
{
    [Header("Walls:")]
    [SerializeField] GameObject _wall1;
    [SerializeField] GameObject _wall2;

    [Header("Fake material:")]
    [SerializeField] Material _fakeWallMaterial;

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
        if(_fadingEffect == null)
        {
            _fadingEffect = StartCoroutine(FadeAlpha());
        }
    }    

    IEnumerator FadeAlpha()
    {
        GameParams.Managers.audioManager.PlayOneShotSpatialized(GameParams.Managers.fmodEvents.SFX_IllusionBreak, transform);


        Material prevWall1Material = _wall1Renderer.material;
        Material prevWall2Material = _wall2Renderer.material;

        _wall1Renderer.material = _fakeWallMaterial;
        _wall2Renderer.material = _fakeWallMaterial;

        Material wall1Material = _wall1Renderer.material;
        Material wall2Material = _wall2Renderer.material;

        while (wall1Material.color.a > 0.1f)
        {
            Color newAlpha = wall1Material.color;
            newAlpha.a -= 0.01f;

            wall1Material.color = newAlpha;
            wall2Material.color = newAlpha;

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1f); // Wait for 1 second before increasing alpha back

        while (wall1Material.color.a < 1f)
        {
            Color newAlpha = wall1Material.color;
            newAlpha.a += 0.01f;

            wall1Material.color = newAlpha;
            wall2Material.color = newAlpha;

            yield return new WaitForFixedUpdate();
        }

        _fadingEffect = null;

        _wall1Renderer.material = prevWall1Material;
        _wall2Renderer.material = prevWall2Material;
    }
}
