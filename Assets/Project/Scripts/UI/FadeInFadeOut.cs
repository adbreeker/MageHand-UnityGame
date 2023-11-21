using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeInFadeOut : MonoBehaviour
{
    public bool fadeInScene = true;
    public float fadingSpeed = 0.05f;
    public GameObject blackoutPrefab;

    void Start()
    {
        if(fadeInScene) StartCoroutine(FadeInScene());
    }

    public void ChangeScene(string sceneName, bool fadeOut = true)
    {
        if(fadeOut) StartCoroutine(FadeOutScene(sceneName));
        else SceneManager.LoadScene(sceneName);
    }

    public void CloseGame()
    {
        StartCoroutine(FadeOutGame());
    }

    IEnumerator FadeInScene()
    {
        GameObject instantiatedBlackoutGameObject = Instantiate(blackoutPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        RawImage blackoutImage = instantiatedBlackoutGameObject.transform.Find("Img").GetComponent<RawImage>();
        float alpha = blackoutImage.color.a;

        alpha = 1;
        while (alpha > 0)
        {
            alpha -= fadingSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }
        Destroy(instantiatedBlackoutGameObject);
    }

    IEnumerator FadeOutScene(string sceneName)
    {
        GameObject instantiatedBlackoutGameObject = Instantiate(blackoutPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        RawImage blackoutImage = instantiatedBlackoutGameObject.transform.Find("Img").GetComponent<RawImage>();
        float alpha = blackoutImage.color.a;

        BackgroundMusic backgroundMusic = FindObjectOfType<BackgroundMusic>();
        float startVolumeDecrease = backgroundMusic.startMusicAS.volume / (1 / fadingSpeed);
        float loopVolumeDecrease = backgroundMusic.loopMusicAS.volume / (1 / fadingSpeed);

        alpha = 0;
        while (alpha < 1)
        {
            backgroundMusic.startMusicAS.volume -= startVolumeDecrease;
            backgroundMusic.loopMusicAS.volume -= loopVolumeDecrease;
            alpha += fadingSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOutGame()
    {
        GameObject instantiatedBlackoutGameObject = Instantiate(blackoutPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        RawImage blackoutImage = instantiatedBlackoutGameObject.transform.Find("Img").GetComponent<RawImage>();
        float alpha = blackoutImage.color.a;

        alpha = 0;
        while (alpha < 1)
        {
            alpha += fadingSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }
}
