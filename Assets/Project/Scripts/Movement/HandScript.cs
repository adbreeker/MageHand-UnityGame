using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using System.Globalization;


public class HandScript : MonoBehaviour
{
    Vector3[] vec;
    Vector3 handPosition;

    public static Vector3 minPoint;
    public static Vector3 maxPoint;

    public GameObject[] handPoints;
    public UDPReceive udpReceive;

    static private Camera mainCamera;
    private float cameraHeight;
    private float cameraWidth;
    public static float z;

    bool running;

    private void Update() 
    {
        Vector3 newPos = RotateAroundPoint(handPosition, mainCamera.transform.position, mainCamera.transform.eulerAngles.y);
        string data = udpReceive.data;
        vec = StringToVector3(data);
        
        if (vec != null)
        {
            for (int i = 0; i < vec.Length; i++)
            {
                handPoints[i].transform.localPosition = vec[i];
            }
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        CalculateNearPlaneBounds();
    }

    private void CalculateNearPlaneBounds()
    {
        Vector3 objectPosition = transform.position;
        Vector3 objectPositionInCameraSpace = mainCamera.transform.InverseTransformPoint(objectPosition);
        float distance = Mathf.Abs(objectPositionInCameraSpace.z);
        float halfHeight = distance * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * mainCamera.aspect;

        float minX = -halfWidth;
        float maxX = halfWidth;
        float minY = -halfHeight;
        float maxY = halfHeight;
        z = objectPosition.z;


        minPoint = mainCamera.transform.TransformPoint(new Vector3(minX, minY, distance));
        maxPoint = mainCamera.transform.TransformPoint(new Vector3(maxX, maxY, distance));

        Debug.Log("minPoint: " + minPoint + ", maxPoint: " + maxPoint);


        if (objectPosition.x >= minPoint.x && objectPosition.x <= maxPoint.x &&
        objectPosition.y >= minPoint.y && objectPosition.y <= maxPoint.y )
        {
            Debug.Log("Object is visible on the camera");
            handPosition = new Vector3(
                objectPosition.x,
                objectPosition.y,
                objectPosition.z);
        } else {
            Debug.Log("Object is not visible on the camera");
            handPosition = new Vector3(
                maxPoint.x - Math.Abs(maxPoint.x + minPoint.x) / 2,
                maxPoint.y - Math.Abs(maxPoint.y + minPoint.y) / 2,
                objectPosition.z);
        }

    }

    public static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, float angle)
    {
        if (angle == 0.0f)
        {
            return point;
        }

        point -= pivot;

        Vector3 axis = Vector3.up;
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);
        point = rotation * point;

        point += pivot;

        return point;
    }


    public static Vector3[] StringToVector3(string sVector)
    {
        string[] vectors = sVector.Split(';');
        
        Vector3[] temp = new Vector3[vectors.Length];
    
        for (int i = 0; i < vectors.Length; i++) {

            string[] coordinates = vectors[i].Split(',');
            
            if (coordinates.Length == 3) {

                float x = maxPoint.x - (Math.Abs(maxPoint.x - minPoint.x)) * float.Parse(coordinates[0], CultureInfo.InvariantCulture);
                float y = maxPoint.y - (Math.Abs(maxPoint.y - minPoint.y)) * float.Parse(coordinates[1], CultureInfo.InvariantCulture);
                float zAxis = z;

                Vector3 position = new Vector3(x, y, zAxis);
                temp[i] = position;
                
            }
        }
        return temp;
    }

}

