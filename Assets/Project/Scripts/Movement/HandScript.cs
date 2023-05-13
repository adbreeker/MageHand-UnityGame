using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;

public class HandScript : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;

    Vector3[] mpPoints;

    Vector3 handPosition;

    public static Vector3 minPoint;
    public static Vector3 maxPoint;

    static private Camera mainCamera;
    private float cameraHeight;
    private float cameraWidth;
    public static float z;

    bool running;

    private void Update() // 1. Get all points as an array or vectors 2. Calculate the rotation in joints on 2 different planes 3. Rotate hand using local variables || IN THIS ORDER 
    {
        Vector3 newPos = RotateAroundPoint(handPosition, mainCamera.transform.position, mainCamera.transform.eulerAngles.y);
        transform.position = newPos; //assigning receivedPos in SendAndReceiveData()
    }

    private void Start()
    {
        mainCamera = Camera.main;
        CalculateNearPlaneBounds();

        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();

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


    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            SendAndReceiveData();
        }
        listener.Stop();
    }


    void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        //---receiving Data from the Host----
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); //Getting data in Bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string

        if (dataReceived != null)
        {
            //---Using received data---
            mpPoints = StringToVector3(dataReceived); //<-- assigning receivedPos value from Python
            handPosition = mpPoints[7];
            print("received pos data, and moved the Cube!");

            //---Sending Data to Host----
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hey I got your message Python! Do You see this massage?"); //Converting string to byte data
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python
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

                float x = maxPoint.x - (Math.Abs(maxPoint.x - minPoint.x)) * float.Parse(coordinates[0]);
                float y = maxPoint.y - (Math.Abs(maxPoint.y - minPoint.y)) * float.Parse(coordinates[1]);
                float zAxis = z;

                Vector3 position = new Vector3(x, y, zAxis);
                temp[i] = position;
            }
        }
        return temp;
    }
    /*
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    */
}