using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMarkBehavior : MonoBehaviour 
{
    [SerializeField] GameObject _impactPrefab;

    private void Start()
    {
        Instantiate(_impactPrefab, transform.position, Quaternion.identity);
        StartCoroutine(IncreaseSize());
    }

    IEnumerator IncreaseSize()
    {
        Vector3 sizeToReach = transform.localScale * 2;

        while (transform.localScale != sizeToReach)
        {
            yield return new WaitForFixedUpdate();
            transform.localScale = Vector3.MoveTowards(transform.localScale, sizeToReach, 0.01f);
        }
    }
}
