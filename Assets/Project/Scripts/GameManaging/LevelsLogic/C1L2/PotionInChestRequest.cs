using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PotionInChestRequest : MonoBehaviour
{
    public BoxCollider chestBounds;

    public GameObject potion;

    public GameObject treasurePrefab;
    public Vector3 treasureTeleportPos;
    public GameObject treasure;

    bool potionPicked = false;
    bool treasureTeleported = false;

    bool isPotionInChest = false;
    bool isTreasureInChest = true;

    private void Start()
    {
        PlayerParams.Controllers.pointsManager.maxPlotPoints += 2;
        PlayerParams.Controllers.pointsManager.minPlotPoints += -3;

        LevelExitCube.OnLevelChange += OnLevelChange;

        treasure = Instantiate(treasurePrefab, new Vector3(0,-10, 0), Quaternion.identity);
    }

    private void Update()
    {
        if(!potionPicked && PlayerParams.Controllers.handInteractions.inHand == potion)
        {
            potionPicked=true;
        }

        if(isPotionInChest && !treasureTeleported)
        {
            treasureTeleported = true;
            treasure.GetComponent<TreasureBehavior>().TeleportTo(treasureTeleportPos, treasure.transform.rotation.eulerAngles.y, null);
        }

        if(potionPicked)
        {
            if(potion != null)
            {
                if (chestBounds.bounds.Contains(potion.transform.position) && potion.transform.parent == null)
                {
                    isPotionInChest = true;
                }
                else
                {
                    isPotionInChest = false;
                }
            }
            if(treasure != null && treasureTeleported)
            {
                if (chestBounds.bounds.Contains(treasure.transform.position) && treasure.transform.parent == null)
                {
                    isTreasureInChest = true;
                }
                else
                {
                    isTreasureInChest = false;
                }
            }
        }
    }

    public void OnLevelChange()
    {
        Debug.Log("OnLevelChange Event handler from " + this.name);
        int points = 0;

        if(potionPicked)
        {
            if(isTreasureInChest && isPotionInChest) { points = 2; Debug.Log("+2 points"); }
            else if (isTreasureInChest || isPotionInChest) { points = -1; Debug.Log("-1 point"); }
            else { points = -3; Debug.Log("-3 points"); }
        }

        PlayerParams.Controllers.pointsManager.plotPoints += points;

        LevelExitCube.OnLevelChange -= OnLevelChange;
    }
}
