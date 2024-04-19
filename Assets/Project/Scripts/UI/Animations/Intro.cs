using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Intro : MonoBehaviour
{
    public List<CanvasGroup> texts;
    public CanvasGroup frame;
    public CanvasGroup continueButton;
    public List<RawImage> pics;
    public string nextLevel = "Chapter_0_Tutorial";

    private bool animationEnded = false;
    private bool skip = false;

    void Start()
    {
        StartCoroutine(IntroAnimation());
    }

    private void Update()
    {
        if (animationEnded && Input.GetKeyDown(KeyCode.Space))
        {
            FindObjectOfType<FadeInFadeOut>().ChangeScene(nextLevel);
            FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption).Play();
            animationEnded = false;
        }

        if (Input.GetKeyDown(KeyCode.Space)) skip = true;
    }


    IEnumerator IntroAnimation()
    {
        if(!skip) yield return new WaitForSeconds(1);

        //Display frame
        float alphaFrame = 0;
        while (alphaFrame < 1)
        {
            alphaFrame += 0.01f;
            frame.alpha = alphaFrame;

            if (!skip) yield return new WaitForFixedUpdate();
        }

        if (!skip) yield return new WaitForSeconds(1);

        //Display all panels
        for (int i = 0; i < pics.Count && i < texts.Count; i++)
        {
            float alpha = 0;
            while (alpha < 1)
            {
                alpha += 0.0025f;
                pics[i].color = new Color(1, 1, 1, alpha);
                texts[i].alpha = alpha;

                if (!skip) yield return new WaitForFixedUpdate();
            }

            if (!skip) yield return new WaitForSeconds(5);
            else if(i != pics.Count - 1) skip = false;
        }

        //Display continue button
        float alphaContinue = 0;
        while (alphaContinue < 1)
        {
            alphaContinue += 0.01f;
            continueButton.alpha = alphaContinue;

            if (!skip) yield return new WaitForFixedUpdate();
        }

        animationEnded = true;
    }
}
