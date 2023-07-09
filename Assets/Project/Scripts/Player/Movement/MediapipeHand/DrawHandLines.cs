using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHandLines : MonoBehaviour //drawing lines between points of hand created with mediapipe
{
    [Header("Lines width")]
    public float width;

    [Header("Hand points")]
    public List<Transform> points;

    LineRenderer lineRenderer;

    void Start() //setting linerenderer parameters
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = points.Count;
    }

    void Update()
    {
        lineRenderer.SetPositions(GetPositions()); //setting new positions for linerendere
    }

    Vector3[] GetPositions() //getting all points positions
    {
        List<Vector3> temp = new List<Vector3>();
        foreach(Transform t in points)
        {
            temp.Add(t.localPosition);
        }
        return temp.ToArray();
    }
}
