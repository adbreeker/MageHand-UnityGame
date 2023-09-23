using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    public TextMeshProUGUI titleObject;
    public TextMeshProUGUI contentObject;

    private GameObject popUp;
    private void Awake()
    {
        popUp = gameObject;
    }

    public void ActivatePopUp(string title, string content, float timeToFadeOut)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            titleObject.text = title;
            titleObject.gameObject.SetActive(true);
        }
        contentObject.text = content;
        StartCoroutine(FadeOutPopUp(timeToFadeOut));
    }


    IEnumerator FadeOutPopUp(float timeToFadeOut)
    {
        yield return new WaitForSeconds(timeToFadeOut);
        while (popUp.GetComponent<CanvasGroup>().alpha > 0)
        {
            popUp.GetComponent<CanvasGroup>().alpha -= 0.007f;
            yield return new WaitForSeconds(0);
        }
        Destroy(popUp);
    }
}
