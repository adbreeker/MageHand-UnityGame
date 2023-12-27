using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistingHallway : MonoBehaviour
{
    [SerializeField] GameObject _rotationCube;

    bool rotated = false;
    Vector3 rotationPos;

    void Start()
    {
        rotationPos = new Vector3(_rotationCube.transform.position.x, PlayerParams.Objects.player.transform.position.y, _rotationCube.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerParams.Objects.player.transform.position == rotationPos && !rotated)
        {
            PlayerParams.Objects.player.transform.rotation *= Quaternion.Euler(0, 180, 0);
            rotated = true;
        }
        if(rotated && PlayerParams.Objects.player.transform.position != rotationPos)
        {
            rotated = false;
        }
    }
}
