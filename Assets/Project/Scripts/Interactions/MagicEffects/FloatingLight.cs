using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingLight : MonoBehaviour
{
    [Header("Floating light duration")]
    public float spellTimeSeconds = 120f;

    GameObject player;

    void Start() //get necessary components, set floating light parent as player, and start floating light duration timer
    {
        player = PlayerParams.Objects.player;
        gameObject.transform.SetParent(player.transform);
        gameObject.transform.localPosition = new Vector3(0, 1.5f, 1);
        StartCoroutine(Timer());
    }

    private void FixedUpdate() //rotate floating light arround player
    {
        gameObject.transform.RotateAround(player.transform.position, new Vector3(0, 1, 0), 2f);
    }

    IEnumerator Timer() //count floating light duration time and destroy floating light after duration 
    {
        yield return new WaitForSeconds(spellTimeSeconds);
        Destroy(gameObject);
    }
}
