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
        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)) skip = true;
        if (error && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
        {
            FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption).Play();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
                Application.Quit();
        }
    }
    IEnumerator Animation()
    {
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

        if (Microphone.devices.Length > 0 && WebCamTexture.devices.Length > 0) SceneManager.LoadScene("Menu");
        else
        {
            if (Microphone.devices.Length <= 0 && WebCamTexture.devices.Length <= 0) errorText.text =
                    "Camera and Microphone were not detected.<br>Those devices are necessary for the game to work properly.<br>Connect them and restart the game.";
            else if (Microphone.devices.Length <= 0) errorText.text =
                    "Microphone was not detected.<br>This device is necessary for the game to work properly.<br>Connect it and restart the game.";
            else if (WebCamTexture.devices.Length <= 0) errorText.text =
                    "Camera was not detected.<br>This device is necessary for the game to work properly.<br>Connect it and restart the game.";


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
