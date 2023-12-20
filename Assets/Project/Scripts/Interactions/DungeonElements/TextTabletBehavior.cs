using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTabletBehavior : MonoBehaviour
{
    public string tabletText = "";
    public Color textColor = Color.white;

    [SerializeField] TextMeshPro textObject;

    // Update is called once per frame
    void Update()
    {
        textObject.text = tabletText;
        textObject.color = textColor;
    }

    private void OnValidate()
    {
        textObject.text = tabletText;
        textObject.color = textColor;
    }
}
