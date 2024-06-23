using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchAfterPlantDestroy : MonoBehaviour
{
    [SerializeField] WallCannonBehavior _mainCannon;
    [SerializeField] GameObject _plant;
    [SerializeField] List<WallCannonBehavior> _secondaryCannons;

    bool changed = false;

    private void Update()
    {
        if(_plant == null && !changed) 
        {
            changed = true;
            StartCoroutine(ChangeCannonsToMainOnly());
        }
    }

    IEnumerator ChangeCannonsToMainOnly()
    {
        yield return new WaitForSeconds(2.0f);

        _mainCannon.LaunchTeleportingMissile();
        _mainCannon.SetLaunching(true);

        yield return new WaitForSeconds(0.5f);

        foreach (WallCannonBehavior behavior in _secondaryCannons)
        {
            behavior.SetLaunching(false);
        }

        Destroy(this);
    }
}
