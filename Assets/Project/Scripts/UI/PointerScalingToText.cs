using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PointerScalingToText : MonoBehaviour
{
    private Vector3 savedSizeDelta = new Vector3(0f, 0f, 0f);
    public float distanceFromText = 102.5f;

    void FixedUpdate()
    {
        ScalePointerToText();
    }
    void Update()
    {
        ScalePointerToText();
    }
    void LateUpdate()
    {
        ScalePointerToText();
    }



    void ScalePointerToText()
    {
        if (transform.parent.GetComponent<TextMeshProUGUI>() != null)
        {
            transform.parent.GetComponent<TextMeshProUGUI>().ForceMeshUpdate();

            if (transform.parent.GetComponent<TextMeshProUGUI>().textBounds.size != savedSizeDelta)
            {
                GetComponent<RectTransform>().sizeDelta = 
                    new Vector2(transform.parent.GetComponent<TextMeshProUGUI>().textBounds.size.x + 
                    distanceFromText, GetComponent<RectTransform>().sizeDelta.y);

                transform.localPosition = new Vector3(0, 0, 0);
                savedSizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
            }
        }
    }
}
