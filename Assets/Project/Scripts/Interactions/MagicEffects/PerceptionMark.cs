using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PerceptionMark : MonoBehaviour
{
    public GameObject perceptionMarkPrefab;

    public int perceptionTime = 10;

    GameObject _perceptionMark = null;


    private void FixedUpdate()
    {
        if (_perceptionMark == null)
        {
            _perceptionMark = Instantiate(perceptionMarkPrefab, transform);
        }

        perceptionTime--;

        if (perceptionTime <= 0)
        {
            Destroy(_perceptionMark);
            Destroy(this);
        }
    }
}
