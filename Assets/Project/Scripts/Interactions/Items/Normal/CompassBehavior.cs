using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassBehavior : ItemBehavior
{
    [Space(10f), Header("Needle object")]
    [SerializeField] GameObject _needle;
    void Update()
    {
        _needle.transform.localRotation = Quaternion.Euler(
            0, -PlayerParams.Objects.player.transform.rotation.eulerAngles.y - 90, 0);
    }

    public override void OnPickUp()
    {
        base.OnPickUp();
        transform.localRotation = Quaternion.Euler(0, -90, 90);
    }
}
