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
        PlayerParams.Controllers.pointsManager.minPlotPoints += -2;
        PlayerParams.Controllers.pointsManager.maxPlotPoints += 1;

        ChapterExitCube.OnLevelChange += OnLevelChange;

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
            treasure.GetComponent<TreasureBehavior>().TeleportTo(treasureTeleportPos, null);
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
            if(treasure != null)
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
        int points = 0;

        if(potionPicked)
        {
            if(isTreasureInChest && isPotionInChest) { points = 1; Debug.Log("1 punkt"); }
            else if (isTreasureInChest || isPotionInChest) { points = -1; Debug.Log("-1 punkt"); }
            else { points = -2; Debug.Log("-2 punkt"); }
        }

        PlayerParams.Controllers.pointsManager.plotPoints += points;
        Debug.Log("Zaktualizowalem punkty");
    }
}
