using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingLight : MonoBehaviour
{
    public float spellTimeSeconds = 30f;
    GameObject player;
    void Start()
    {
        player = GetComponentInParent<PlayerMovement>().gameObject;
        gameObject.transform.SetParent(player.transform);
        gameObject.transform.localPosition = new Vector3(0, 1.5f, 1);
        StartCoroutine(Timer());
    }

    private void FixedUpdate()
    {
        gameObject.transform.RotateAround(player.transform.position, new Vector3(0, 1, 0), 2f);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(spellTimeSeconds);
        Destroy(gameObject);
    }
}
