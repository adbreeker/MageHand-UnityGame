using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGestureIcon : MonoBehaviour
{
    [Header("Icon time")]
    public int iconTime = 5;

    [Header("Objects")]
    public Texture icon;
    public GameObject gestureIconPrefab;
    private Texture iconChecker;
    private GameObject instantiatedGestureIcon;
    private GetObjectsNearHand getObjectsNearHand;

    void Start() //spawn icon
    {
        instantiatedGestureIcon = Instantiate(gestureIconPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, 
            PlayerParams.Objects.hand.transform.Find("Points").Find("Point (0) Wrist"));

        instantiatedGestureIcon.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        instantiatedGestureIcon.GetComponent<Canvas>().worldCamera = PlayerParams.Objects.uiCamera;
        instantiatedGestureIcon.transform.Find("Icon").GetComponent<RawImage>().texture = icon;
        iconChecker = icon;
        instantiatedGestureIcon.transform.localPosition = new Vector3(-2.5f, 4.5f, -1);
        instantiatedGestureIcon.transform.rotation = new Quaternion(0, 0, 0, 0);

        getObjectsNearHand = PlayerParams.Objects.hand.GetComponent<GetObjectsNearHand>();
    }

    void FixedUpdate() //counting time to stop showing icon
    {
        /*
        instantiatedGestureIcon.transform.localPosition =
            (getObjectsNearHand.wristPoint.position +
            getObjectsNearHand.indexFingerKnucklePoint.position +
            getObjectsNearHand.smallFingerKnucklePoint.position) / 3f;

        instantiatedGestureIcon.transform.localPosition = new Vector3(instantiatedGestureIcon.transform.position.x, 
            instantiatedGestureIcon.transform.position.y, 
            -1);
        */

        iconTime--;
        if (iconTime <= 0)
        {
            DestroyImmediate(instantiatedGestureIcon);
            DestroyImmediate(this);
        }

        if(iconChecker.name != icon.name)
        {
            instantiatedGestureIcon.transform.Find("Icon").GetComponent<RawImage>().texture = icon;
            iconChecker = icon;
        }
    }
}
