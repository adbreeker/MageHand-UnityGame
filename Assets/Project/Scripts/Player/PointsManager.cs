using UnityEngine;

public class PointsManager : MonoBehaviour
{
    [Header("Player stats")]
    public int plotPoints = 0;
    public int foundSecrets = 0;
    public int currency = 0;
    [Header("Game values")]
    public int maxPlotPoints = 0;
    public int minPlotPoints = 0;
    public int maxFoundSecrets = 0;
    public int maxCurrency = 0;
}