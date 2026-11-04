using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScrollPassage : MonoBehaviour
{
    [Header("Scroll")]
    [SerializeField] GameObject _scroll;

    [Header("Bars passage")]
    [SerializeField] OpenBarsPassage _bars;

    private void Start() 
    {
        HandInteractions.OnItemPickUp += OpenPassageAfterPickingScroll;
    }

    void OpenPassageAfterPickingScroll(GameObject scroll)
    {
        if (scroll == _scroll)
        {
            HandInteractions.OnItemPickUp -= OpenPassageAfterPickingScroll;
            _bars.Interaction();
        }
    }
}
