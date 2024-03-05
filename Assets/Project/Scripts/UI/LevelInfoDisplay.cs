using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInfoDisplay : MonoBehaviour
{
    public int secretsOnLevel = 0;
    public GameObject displayPrefab;

    private AudioSource sound;
    private AudioSource sound1;
    private GameObject instantiatedDisplay;
    private TextMeshProUGUI levelName;
    private TextMeshProUGUI secretsNumber;
    private CanvasGroup wholeGroup;
    void Start()
    {
        sound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_LevelInfoSound);
        sound1 = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_LevelInfoSound);

        instantiatedDisplay = Instantiate(displayPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

        levelName = instantiatedDisplay.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        secretsNumber = instantiatedDisplay.transform.Find("Secrets").GetComponent<TextMeshProUGUI>();
        wholeGroup = instantiatedDisplay.GetComponent<CanvasGroup>();

        string name = SceneManager.GetActiveScene().name.Replace("_", " ");
        int firstSpaceIndex = name.IndexOf(' ');
        int secondSpaceIndex = name.IndexOf(' ', firstSpaceIndex + 1);
        if (firstSpaceIndex >= 0 && secondSpaceIndex > firstSpaceIndex)
        {
            name = name.Substring(0, secondSpaceIndex) + ": " + name.Substring(secondSpaceIndex + 1);
        }
        levelName.text = name;

        secretsNumber.text = "Secrets: " + secretsOnLevel;

        StartCoroutine(InfoAnimation());
    }

    IEnumerator InfoAnimation()
    {
        float alphaWhole = 0;
        while (alphaWhole < 1)
        {
            alphaWhole += 0.01f;
            wholeGroup.alpha = alphaWhole;
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(1);

        sound.Play();
        float alphaName = 0;
        while (alphaName < 1)
        {
            alphaName += 0.05f;
            levelName.alpha = alphaName;
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(1);

        sound1.Play();
        float alphaSecret = 0;
        while (alphaSecret < 1)
        {
            alphaSecret += 0.05f;
            secretsNumber.alpha = alphaSecret;
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(3f);

        alphaWhole = 1;
        while (alphaWhole > 0)
        {
            alphaWhole -= 0.01f;
            alphaName -= 0.05f;
            wholeGroup.alpha = alphaWhole;
            levelName.alpha = alphaName;
            secretsNumber.alpha = alphaName;
            yield return new WaitForSeconds(0);
        }

        Destroy(sound, sound.clip.length);
        Destroy(sound1, sound1.clip.length);
        Destroy(instantiatedDisplay);
    }
}
