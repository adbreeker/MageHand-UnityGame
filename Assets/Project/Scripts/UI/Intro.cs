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


    public string nextLevel = "Level_0_Tutorial";
    private bool animationEnded = false;

    void Start()
    {
        StartCoroutine(Animation());
    }

    private void Update()
    {
        if (animationEnded && Input.GetKeyDown(KeyCode.Space))
        {
            FindObjectOfType<FadeInFadeOut>().ChangeScene(nextLevel);
        }
    }


    IEnumerator Animation()
    {
        yield return new WaitForSeconds(1);

        //Display frame
        float alphaFrame = 0;
        while (alphaFrame < 1)
        {
            alphaFrame += 0.01f;
            frame.alpha = alphaFrame;
            yield return new WaitForSeconds(0);
        }
        yield return new WaitForSeconds(0);


        //Display all panels
        for (int i = 0; i < pics.Count && i < texts.Count; i++)
        {

            float alpha = 0;
            while (alpha < 1)
            {
                alpha += 0.0025f;
                pics[i].color = new Color(1, 1, 1, alpha);
                texts[i].alpha = alpha;
                yield return new WaitForSeconds(0);
            }
            yield return new WaitForSeconds(0);
        }

        //Display continue button
        float alphaContinue = 0;
        while (alphaContinue < 1)
        {
            alphaContinue += 0.025f;
            continueButton.alpha = alphaContinue;
            yield return new WaitForSeconds(0);
        }
        animationEnded = true;
    }
}
