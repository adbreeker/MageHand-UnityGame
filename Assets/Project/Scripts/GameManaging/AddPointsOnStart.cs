using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPointsOnStart : MonoBehaviour
{
    public int maxPlotPointsToAdd = 0;
    public int minPlotPointsToAdd = 0;
    [TextArea(3, 10)]
    public string whyAdded = "";
    private void Start()
    {
        PlayerParams.Controllers.pointsManager.maxPlotPoints += maxPlotPointsToAdd;
        PlayerParams.Controllers.pointsManager.minPlotPoints += minPlotPointsToAdd;
    }
}
