using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PerceptionMark : MonoBehaviour
{
    public int perceptionTime = 10;

    GameObject _perceptionMarkPrefab;
    Color _markColor;

    GameObject _perceptionMark = null;

    public void Initialize(GameObject perceptionMarkPrefab, Color markColor)
    {
        _perceptionMarkPrefab = perceptionMarkPrefab;
        _markColor = markColor;
    }

    private void FixedUpdate()
    {
        if (_perceptionMark == null)
        {
            _perceptionMark = Instantiate(_perceptionMarkPrefab, transform);
            _perceptionMark.GetComponent<ParticlesColor>().ChangeColorOfEffect(_markColor);
        }

        perceptionTime--;

        if (perceptionTime <= 0)
        {
            Destroy(_perceptionMark);
            Destroy(this);
        }
    }
}
