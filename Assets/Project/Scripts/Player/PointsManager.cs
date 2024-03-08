using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public int plotPoints = 0;
    public int maxPlotPoints = 0;

    public int foundSecrets = 0;
    public int maxFoundSecrets = 0;

    
    public void AddPlotPoints(int points)
    {
        if (plotPoints + points < 0) plotPoints = 0;
        else plotPoints += points;
    }

    public void AddMaxPlotPoints(int points)
    {
        if (maxPlotPoints + points < 0) maxPlotPoints = 0;
        else maxPlotPoints += points;
    }
}