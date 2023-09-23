using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpActivate : MonoBehaviour
{
    [Header("Note content")]
    public string title;
    public string content;
    public float timeToFadeOut = 2;

    [Header("Destroy on pick up")]
    public bool destroy = false;

    [Header("Prefabs")]
    public GameObject popUpPrefab;

    public void OnPickUp() //instantiate pop up prefab
    {
        GameObject popUp = Instantiate(popUpPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        popUp.GetComponent<PopUp>().ActivatePopUp(title, content, timeToFadeOut);

        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
