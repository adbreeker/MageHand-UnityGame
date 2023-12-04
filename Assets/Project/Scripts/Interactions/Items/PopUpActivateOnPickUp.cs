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

    public void OnPickUp() //instantiate pop up prefab
    {
        FindObjectOfType<HUD>().SpawnPopUp(title, content, timeToFadeOut, timeOfFadingOut);

        if (destroy) Destroy(gameObject);
    }
}
