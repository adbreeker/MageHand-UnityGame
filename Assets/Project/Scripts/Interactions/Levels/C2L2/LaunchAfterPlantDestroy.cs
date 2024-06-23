using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchAfterPlantDestroy : MonoBehaviour
{
    [SerializeField] WallCannonBehavior _mainCannon;
    [SerializeField] GameObject _plant;
    [SerializeField] List<WallCannonBehavior> _secondaryCannons;

    private void Update()
    {
        if(_plant == null) 
        {
            ChangeCannonsToMainOnly();
            Destroy(this);
        }
    }

    void ChangeCannonsToMainOnly()
    {
        _mainCannon.LaunchTeleportingMissile();
        _mainCannon.SetLaunching(true);

        foreach(WallCannonBehavior behavior in _secondaryCannons) 
        {
            behavior.SetLaunching(false);
        }
    }
}
