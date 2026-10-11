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
    private const int PointsBufferSize = 700;
    private const int GestureBufferSize = 12;
    private const float ReconnectInterval = 1f;

    //position of hand, and on all points in hand
    Vector3[] vec;
    Vector3 handPosition;

    private MemoryMappedFile pointsMapping;
    private MemoryMappedViewStream pointsStream;
    private MemoryMappedFile gestureMapping;
    private MemoryMappedViewStream gestureStream;
    private readonly byte[] pointsBuffer = new byte[PointsBufferSize];
    private readonly byte[] gestureBuffer = new byte[GestureBufferSize];
    private float nextConnectionAttemptTime;

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
        if (!EnsureMemoryMappedStreams())
        {
            return;
        }

        if (!TryReadFrame(pointsStream, pointsBuffer) || !TryReadFrame(gestureStream, gestureBuffer))
        {
            ReleaseMemoryMappedStreams();
            nextConnectionAttemptTime = Time.unscaledTime + ReconnectInterval;
            return;
        }

        string data = Encoding.UTF8.GetString(pointsBuffer, 0, PointsBufferSize);
        gesture = Encoding.UTF8.GetString(gestureBuffer, 0, GestureBufferSize).Split(';')[0];
        vec = StringToVector3(data);

        if (handPoints == null || vec == null || vec.Length != handPoints.Length)
        {
            return;
        }

        for (int i = 0; i < vec.Length; i++)
        {
            handPoints[i].transform.localPosition = vec[i];
        }
    }

    protected virtual void OnDisable()
    {
        ReleaseMemoryMappedStreams();
    }

    private bool EnsureMemoryMappedStreams()
    {
        if (pointsStream != null && gestureStream != null)
        {
            return true;
        }

        if (Time.unscaledTime < nextConnectionAttemptTime)
        {
            return false;
        }

        nextConnectionAttemptTime = Time.unscaledTime + ReconnectInterval;

        if (pointsStream == null)
        {
            TryOpenStream("magehand_points", out pointsMapping, out pointsStream);
        }

        if (gestureStream == null)
        {
            TryOpenStream("magehand_gestures", out gestureMapping, out gestureStream);
        }

        return pointsStream != null && gestureStream != null;
    }

    private static bool TryOpenStream(string name, out MemoryMappedFile mapping, out MemoryMappedViewStream stream)
    {
        mapping = null;
        stream = null;

        try
        {
            mapping = MemoryMappedFile.OpenExisting(name);
            stream = mapping.CreateViewStream();
            return true;
        }
        catch
        {
            stream?.Dispose();
            mapping?.Dispose();
            mapping = null;
            stream = null;
            return false;
        }
    }

    private static bool TryReadFrame(MemoryMappedViewStream stream, byte[] buffer)
    {
        try
        {
            stream.Position = 0;
            int bytesRead = 0;
            while (bytesRead < buffer.Length)
            {
                int count = stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
                if (count == 0)
                {
                    return false;
                }

                bytesRead += count;
            }

            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    private void ReleaseMemoryMappedStreams()
    {
        pointsStream?.Dispose();
        pointsMapping?.Dispose();
        gestureStream?.Dispose();
        gestureMapping?.Dispose();

        pointsStream = null;
        pointsMapping = null;
        gestureStream = null;
        gestureMapping = null;
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
        Vector3[] temp = new Vector3[vectors.Length - 1];

        for (int i = 0; i < vectors.Length - 1; i++)
        {

            string[] coordinates = vectors[i].Split(',');

            if (coordinates.Length == 3)
            {

                if (!float.TryParse(coordinates[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float normalizedX) ||
                    !float.TryParse(coordinates[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float normalizedY))
                {
                    return null;
                }

                float x = (maxPoint.x - (Math.Abs(maxPoint.x - minPoint.x)) * normalizedX) * 2;
                float y = (maxPoint.y - (Math.Abs(maxPoint.y - minPoint.y)) * normalizedY) * 2;
                float zAxis = z; //- 5.0f*float.Parse(coordinates[2], CultureInfo.InvariantCulture); //zAxis is for some reason moved 1 forward

                Vector3 position = new Vector3(x, y, zAxis);
                temp[i] = position;

            }
        }
        return temp;
    }

}
