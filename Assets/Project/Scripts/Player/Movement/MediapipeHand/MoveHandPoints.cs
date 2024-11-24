using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;

public class MoveHandPoints : MonoBehaviour //move points of hand generated with mediapipe
{
    //position of hand, and on all points in hand
    Vector3[] vec;
    Vector3 handPosition;

    [Header("All hand points objects")]
    public GameObject[] handPoints;

    [Header("Current gesture")]
    public string gesture;

    private Vector3 minPoint;
    private Vector3 maxPoint;
    private float z;

    protected virtual void Start()
    {
        CalculateNearPlaneBounds();
    }

    protected virtual void Update()
    {
        //Vector3 newPos = RotateAroundPoint(handPosition, PlayerParams.Objects.playerCamera.transform.position, PlayerParams.Objects.playerCamera.transform.eulerAngles.y);
        MemoryMappedFile mmf_points;
        try
        {
            mmf_points = MemoryMappedFile.OpenExisting("magehand_points");
        }
        catch
        {
            return;
        }
        
        MemoryMappedViewStream stream_points = mmf_points.CreateViewStream();
        BinaryReader reader_points = new BinaryReader(stream_points);
        byte[] framePoints = reader_points.ReadBytes(700);
        string data = System.Text.Encoding.UTF8.GetString(framePoints, 0, 700);
        vec = StringToVector3(data);

        if (vec != null)
        {
            for (int i = 0; i < vec.Length; i++)
            {
                handPoints[i].transform.localPosition = vec[i];
            }
        }
    }

    private void CalculateNearPlaneBounds()
    {
        Vector3 objectPosition = transform.localPosition;
        float distance = Mathf.Abs(objectPosition.z);
        float halfHeight = distance * Mathf.Tan(PlayerParams.Objects.playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * PlayerParams.Objects.playerCamera.aspect;

        float minX = -halfWidth;
        float maxX = halfWidth;
        float minY = -halfHeight;
        float maxY = halfHeight;
        z = objectPosition.z;


        minPoint = new Vector3(minX, minY, distance);
        maxPoint = new Vector3(maxX, maxY, distance);

        //Debug.Log("minPoint: " + minPoint + ", maxPoint: " + maxPoint);


        if (objectPosition.x >= minPoint.x && objectPosition.x <= maxPoint.x &&
        objectPosition.y >= minPoint.y && objectPosition.y <= maxPoint.y)
        {
            //Debug.Log("Object is visible on the camera");
            handPosition = new Vector3(
                objectPosition.x,
                objectPosition.y,
                objectPosition.z);
        }
        else
        {
            //Debug.Log("Object is not visible on the camera");
            handPosition = new Vector3(
                maxPoint.x - Math.Abs(maxPoint.x + minPoint.x) / 2,
                maxPoint.y - Math.Abs(maxPoint.y + minPoint.y) / 2,
                objectPosition.z);
        }

    }

    Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, float angle)
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


    Vector3[] StringToVector3(string sVector)
    {
        string[] vectors = sVector.Split(';');

        // Labels of gestures:
        // None, Closed_Fist, Open_Palm, Pointing_Up, Thumb_Down, Thumb_Up, Victory, ILoveYou
        MemoryMappedFile mmf_gesture = MemoryMappedFile.OpenExisting("magehand_gestures");
        MemoryMappedViewStream stream_gesture = mmf_gesture.CreateViewStream();
        BinaryReader reader_gesture = new BinaryReader(stream_gesture);
        byte[] frameGesture = reader_gesture.ReadBytes(12);
        gesture = System.Text.Encoding.UTF8.GetString(frameGesture, 0, 12).Split(';')[0];

        Vector3[] temp = new Vector3[vectors.Length - 1];

        for (int i = 0; i < vectors.Length - 1; i++)
        {

            string[] coordinates = vectors[i].Split(',');

            if (coordinates.Length == 3)
            {

                float x = (maxPoint.x - (Math.Abs(maxPoint.x - minPoint.x)) * float.Parse(coordinates[0], CultureInfo.InvariantCulture)) * 2;
                float y = (maxPoint.y - (Math.Abs(maxPoint.y - minPoint.y)) * float.Parse(coordinates[1], CultureInfo.InvariantCulture)) * 2;
                float zAxis = z; //- 5.0f*float.Parse(coordinates[2], CultureInfo.InvariantCulture); //zAxis is for some reason moved 1 forward

                Vector3 position = new Vector3(x, y, zAxis);
                temp[i] = position;

            }
        }
        return temp;
    }

}
