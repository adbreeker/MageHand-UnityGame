using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeInFadeOut : MonoBehaviour
{
    public bool fadeInScene = true;
    public GameObject blackoutPrefab;

    public float fadeInSpeed = 0.025f;
    public float fadeOutSpeed = 0.025f;

    void Start()
    {
        if(fadeInScene) StartCoroutine(FadeInScene());
    }

    public void ChangeScene(string sceneName, bool fadeOut = true, bool fadeOutAndChangeMusic = true)
    {
        FindObjectOfType<BackgroundMusic>().modifyBackgroundMusicVolume = true;
        if (fadeOut) StartCoroutine(FadeOutScene(sceneName, fadeOutAndChangeMusic));
        else SceneManager.LoadScene(sceneName);
    }

    public void CloseGame()
    {
        FindObjectOfType<BackgroundMusic>().modifyBackgroundMusicVolume = true;
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
            alpha -= fadeInSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }
        Destroy(instantiatedBlackoutGameObject);
    }

    IEnumerator FadeOutScene(string sceneName, bool fadeOutAndChangeMusic)
    {
        GameObject instantiatedBlackoutGameObject = Instantiate(blackoutPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        RawImage blackoutImage = instantiatedBlackoutGameObject.transform.Find("Img").GetComponent<RawImage>();


        BackgroundMusic backgroundMusic = FindObjectOfType<BackgroundMusic>();
        float startVolumeDecrease = backgroundMusic.startMusicAS.volume * fadeOutSpeed;
        float loopVolumeDecrease = backgroundMusic.loopMusicAS.volume * fadeOutSpeed;

        float alpha = 0;

        FindAnyObjectByType<SoundManager>().FadeOutSFXs();

        while (alpha < 1)
        {
            if(fadeOutAndChangeMusic)
            {
                backgroundMusic.startMusicAS.volume -= startVolumeDecrease;
                backgroundMusic.loopMusicAS.volume -= loopVolumeDecrease;
            }
            alpha += fadeOutSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }

        if (fadeOutAndChangeMusic) Destroy(backgroundMusic.gameObject);

        if (PlayerParams.Controllers.pauseMenu != null) PlayerParams.Controllers.pauseMenu.freezeTime = false;
        yield return new WaitForFixedUpdate();

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOutGame()
    {
        GameObject instantiatedBlackoutGameObject = Instantiate(blackoutPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        RawImage blackoutImage = instantiatedBlackoutGameObject.transform.Find("Img").GetComponent<RawImage>();

        BackgroundMusic backgroundMusic = FindObjectOfType<BackgroundMusic>();
        float startVolumeDecrease = backgroundMusic.startMusicAS.volume * fadeOutSpeed;
        float loopVolumeDecrease = backgroundMusic.loopMusicAS.volume * fadeOutSpeed;

        float alpha = 0;
        while (alpha < 1)
        {
            backgroundMusic.startMusicAS.volume -= startVolumeDecrease;
            backgroundMusic.loopMusicAS.volume -= loopVolumeDecrease;
            alpha += fadeOutSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);
        }

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }
}
