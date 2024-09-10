using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoEnding : MonoBehaviour
{
    public RectTransform creditsText;
    public GameObject fadePrefab;

    private bool end = false;
    private bool ending = false;
    void Start()
    {
        StartCoroutine(Animation());
    }

    private void Update()
    {
        if (end && !ending && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
            GameParams.Managers.fadeInOutManager.CloseGame();
            ending = true;
        }


        if (!end && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
        {
            end = true;
            StartCoroutine(Blink());
        }
    }

    IEnumerator Animation()
    {
        yield return new WaitForSeconds(3.5f);

        float startPosition = creditsText.localPosition.y;
        while (creditsText.localPosition.y < -startPosition)
        {
            creditsText.localPosition = new Vector3(creditsText.localPosition.x, creditsText.localPosition.y + 1f, creditsText.localPosition.z);
            yield return new WaitForFixedUpdate();
        }

        GameParams.Managers.fadeInOutManager.CloseGame();
    }

    IEnumerator Blink()
    {
        GameObject fadeObject = Instantiate(fadePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        RawImage fade = fadeObject.transform.Find("Img").GetComponent<RawImage>();
        fade.color = new Color(0, 0, 0, 0);

        while (fade.color.a < 0.5f)
        {
            fade.color = new Color(0, 0, 0, fade.color.a + 0.05f);
            yield return new WaitForFixedUpdate();
        }

        while (fade.color.a > 0f)
        {
            fade.color = new Color(0, 0, 0, fade.color.a - 0.05f);
            yield return new WaitForFixedUpdate();
        }

        Destroy(fadeObject);
    }

}
