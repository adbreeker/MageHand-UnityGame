using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBehavior : ItemBehavior
{
    [Space(10f), Header("Treasure value")]
    public int value;

    private void Start()
    {
        PlayerParams.Controllers.pointsManager.maxCurrency += value;
    }

    public override void OnPickUp()
    {
        base.OnPickUp();
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }
}
