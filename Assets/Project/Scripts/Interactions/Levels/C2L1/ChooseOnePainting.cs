using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseOnePainting : MonoBehaviour
{
    public GameObject painting1;
    public GameObject painting2;
    public GameObject painting3;

    public Vector3 tpDestination;

    public OpenBarsPassage passage;
    public LeverBehavior lever;
    public GameObject cubeLeverTrigger;
    
    bool leverTriggered = false;

    private void Awake()
    {
        PlayerParams.Controllers.pointsManager.maxPlotPoints += 2;
        PlayerParams.Controllers.pointsManager.minPlotPoints += -2;
    }

    private void Update()
    {
        if(!leverTriggered)
        {
            Vector3 cubePosWithPlayer = cubeLeverTrigger.transform.position;
            cubePosWithPlayer.y = 1;

            if(cubePosWithPlayer == PlayerParams.Controllers.playerMovement.currentTilePos && lever.leverON)
            {
                leverTriggered = true;
                lever.OnClick();
            }
        }
    }

    public void ChosenPaintingInteraction(GameObject painting)
    {
        if(painting == painting1)
        {
            PlayerParams.Controllers.pointsManager.plotPoints += -2;
            PlayerParams.Controllers.playerMovement.TeleportTo(tpDestination, null);
        }
        if(painting == painting2)
        {
            PlayerParams.Controllers.pointsManager.plotPoints += 1;
            PlayerParams.Controllers.playerMovement.TeleportTo(tpDestination, null);
        }
        if(painting == painting3)
        {
            PlayerParams.Controllers.pointsManager.plotPoints += 2;
            PlayerParams.Controllers.playerMovement.TeleportTo(tpDestination, null);
        }

        passage.Interaction();
    }
}
