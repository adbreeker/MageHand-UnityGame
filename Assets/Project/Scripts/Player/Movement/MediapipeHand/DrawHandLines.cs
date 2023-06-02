using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHandLines : MonoBehaviour
{
    LineRenderer lineRenderer;

    public float width;

    public List<Transform> points;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = points.Count;
    }

    void Update()
    {
        lineRenderer.SetPositions(GetPositions());
    }

    Vector3[] GetPositions()
    {
        List<Vector3> temp = new List<Vector3>();
        foreach(Transform t in points)
        {
            temp.Add(t.localPosition);
        }
        return temp.ToArray();
    }
}
