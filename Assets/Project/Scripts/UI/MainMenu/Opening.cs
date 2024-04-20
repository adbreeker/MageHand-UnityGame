using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Opening : MonoBehaviour
{
    public RawImage fade;
    public GameObject info;
    public TextMeshProUGUI errorText;

    private float alpha;
    private bool error = false;
    void Start()
    {
        alpha = fade.color.a;
        StartCoroutine(Animation());
    }

    private void Update()
    {
        if (error && Input.GetKeyDown(KeyCode.Space))
        {
            GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_SelectOption).Play();
            GameParams.Managers.fadeInOutManager.CloseGame();
        }
    }
    IEnumerator Animation()
    {
        //Check if saves directory exists, if not create
        ProgressSaving.CreateSavesDirectory();

        while (alpha > 0)
        {
            alpha -= 0.01f;
            fade.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(5);

        while (alpha < 1)
        {
            alpha += 0.01f;
            fade.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }

        if (WebCamTexture.devices.Length > 0) GameParams.Managers.fadeInOutManager.ChangeScene("Menu");
        else
        {
            info.SetActive(false);
            while (alpha > 0)
            {
                alpha -= 0.01f;
                fade.color = new Color(0, 0, 0, alpha);
                yield return new WaitForSeconds(0);
            }
            error = true;
        }
    }
}
