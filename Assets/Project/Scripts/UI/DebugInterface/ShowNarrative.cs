using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class ShowNarrative : MonoBehaviour
{
    private Text narrativeText;
    float timeElapsed;

    private void Awake()
    {
        narrativeText = GetComponent<Text>();
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= 1f)
        {
            timeElapsed = 0f;

            int secretsAllScene = GameParams.Managers.levelInfoManager.secretsOnLevel;
            int secretsFoundScene = GameParams.Managers.levelInfoManager.foundSecretsOnLevel;
            int secretsAll = PlayerParams.Controllers.pointsManager.maxFoundSecrets;
            int secretsFound = PlayerParams.Controllers.pointsManager.foundSecrets;

            int pointsCurrent = PlayerParams.Controllers.pointsManager.plotPoints;
            int pointMin = PlayerParams.Controllers.pointsManager.minPlotPoints;
            int pointMax = PlayerParams.Controllers.pointsManager.maxPlotPoints;

            int currencyAll = PlayerParams.Controllers.pointsManager.maxCurrency;
            int currencyCurrent = PlayerParams.Controllers.pointsManager.currency;

            narrativeText.text = $"Secrets found: {secretsFoundScene}/{secretsAllScene} (Total: {secretsFound}/{secretsAll})\n" +
                                 $"Plot points: {pointsCurrent}/({pointMin}-{pointMax})\n" +
                                 $"Currency: {currencyCurrent}/{currencyAll}";
        }
    }
}