using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    public TextMeshProUGUI titleObject;
    public TextMeshProUGUI contentObject;

    private AudioSource popUpSound;

    public void ActivatePopUp(string title, string content, float timeToFadeOut, float timeOfFadingOut)
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("HUD").transform.Find("PopUpContainer").transform);

        if (!string.IsNullOrWhiteSpace(title))
        {
            titleObject.text = title;
            titleObject.gameObject.SetActive(true);
        }
        contentObject.text = content;

        popUpSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_PopUp);
        popUpSound.Play();

        StartCoroutine(FadeOutPopUp(timeToFadeOut, timeOfFadingOut));
    }


    IEnumerator FadeOutPopUp(float timeToFadeOut, float timeOfFadingOut)
    {
        yield return new WaitForSeconds(timeToFadeOut);
        while (GetComponent<CanvasGroup>().alpha > 0)
        {
            GetComponent<CanvasGroup>().alpha -= timeOfFadingOut;
            yield return new WaitForSeconds(0);
        }
        Destroy(popUpSound.gameObject);
        Destroy(gameObject);
    }
}
