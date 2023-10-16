using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpActivateOnPickUp : MonoBehaviour
{
    [Header("Pop up content")]
    public string title;
    [TextArea(1, 2)]
    public string content;
    public float timeToFadeOut = 2;
    public float timeOfFadingOut = 0.007f;

    [Header("Destroy on pick up")]
    public bool destroy = false;

    [Header("Prefabs")]
    public GameObject popUpPrefab;

    public void OnPickUp() //instantiate pop up prefab
    {
        GameObject popUp = Instantiate(popUpPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        popUp.GetComponent<PopUp>().ActivatePopUp(title, content, timeToFadeOut, timeOfFadingOut);

        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
