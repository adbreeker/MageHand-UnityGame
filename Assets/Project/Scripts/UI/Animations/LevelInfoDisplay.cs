using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInfoDisplay : MonoBehaviour
{
    public int secretsOnLevel = 0;
    public int foundSecretsOnLevel = 0;
    public GameObject displayPrefab;

    public float timeToFadeOut = 2;
    public float timeOfFadingOut = 0.007f;

    private GameObject instantiatedDisplay;
    private TextMeshProUGUI levelName;
    private TextMeshProUGUI secretsNumber;
    private CanvasGroup wholeGroup;

    void Start()
    {
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
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1);

        RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_LevelInfo);
        float alphaName = 0;
        while (alphaName < 1)
        {
            alphaName += 0.05f;
            levelName.alpha = alphaName;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1);

        RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_LevelInfo);
        float alphaSecret = 0;
        while (alphaSecret < 1)
        {
            alphaSecret += 0.05f;
            secretsNumber.alpha = alphaSecret;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(5f);

        alphaWhole = 1;
        while (alphaWhole > 0)
        {
            alphaWhole -= 0.01f;
            alphaName -= 0.025f;
            wholeGroup.alpha = alphaWhole;
            levelName.alpha = alphaName;
            secretsNumber.alpha = alphaName;
            yield return new WaitForFixedUpdate();
        }
        Destroy(instantiatedDisplay);
    }

    public void AddSecretOnLevel()
    {
        secretsOnLevel += 1;
        PlayerParams.Controllers.pointsManager.maxFoundSecrets += 1;
    }

    public void SecretFoundPopUp()
    {
        RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_SecretFound);
        PlayerParams.Controllers.pointsManager.foundSecrets += 1;
        foundSecretsOnLevel += 1;
        string text = "Secret found!<br>" + foundSecretsOnLevel + "/" + secretsOnLevel;
        PlayerParams.Controllers.HUD.SpawnPopUp(text, timeToFadeOut, timeOfFadingOut, false);
    }
}
