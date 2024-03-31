using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassBehavior : MonoBehaviour
{
    [SerializeField] GameObject _needle;
    void Update()
    {
        _needle.transform.localRotation = Quaternion.Euler(
            0, -PlayerParams.Objects.player.transform.rotation.eulerAngles.y - 90, 0);
    }

    public void OnPickUp()
    {
        transform.localRotation = Quaternion.Euler(0, -90, 90);
    }
}
