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
    Vector3 receivedPos;

    private  Camera mainCamera;
    private float cameraHeight;
    private float cameraWidth;
    public static float maxLeft;
    public static float maxRight;
    public static float maxBot;
    public static float maxTop; //this method can be used while handling z-coordinate too, but I don't think it's needed (yet)
    public static float z;
    public Bounds _nearPlaneBounds;

    bool running;

    private void Update()
    {
        transform.position = receivedPos; //assigning receivedPos in SendAndReceiveData()
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
        float distance = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        float height = 2.0f * distance * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float width = height * mainCamera.aspect;

        Vector3 objectPosition = transform.position;
        Vector3 cameraPosition = mainCamera.transform.position;

        //account for object position
        float left = cameraPosition.x - width * 0.5f - objectPosition.x;
        float right = cameraPosition.x + width * 0.5f - objectPosition.x;
        float bottom = cameraPosition.y - height * 0.5f - objectPosition.y;
        float top = cameraPosition.y + height * 0.5f - objectPosition.y;

        maxRight = objectPosition.x + right;
        maxLeft = objectPosition.x + left;
        maxBot = objectPosition.y + bottom;
        maxTop = objectPosition.y + top;
        z = objectPosition.z;

        if (objectPosition.x >= maxLeft && objectPosition.x <= maxRight &&
            objectPosition.y >= maxBot && objectPosition.y <= maxTop )
        {
            // The object is visible on the camera
            Debug.Log("Object is visible on the camera");
            receivedPos = new Vector3(
                objectPosition.x,
                objectPosition.y,
                objectPosition.z);
        } else {
            Debug.Log("Object is not visible on the camera");
            receivedPos = new Vector3(
                maxRight - (Math.Abs(maxRight) + Math.Abs(maxLeft))/2,
                maxTop - (Math.Abs(maxTop) + Math.Abs(maxBot))/2,
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
            receivedPos = StringToVector3(dataReceived); //<-- assigning receivedPos value from Python
            print("received pos data, and moved the Cube!");

            //---Sending Data to Host----
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hey I got your message Python! Do You see this massage?"); //Converting string to byte data
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python
        }
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // calculate new points
        float xCoordinate = - Math.Abs(maxRight) + (Math.Abs(maxLeft) + Math.Abs(maxRight)) * float.Parse(sArray[0]);
        UnityEngine.Debug.Log("New x should be: " + xCoordinate);
        float yCoordinate = - Math.Abs(maxTop) + (Math.Abs(maxBot) + Math.Abs(maxTop)) * float.Parse(sArray[1]);
        UnityEngine.Debug.Log("New y should be: " + yCoordinate);
        float zCoordinate = z - float.Parse(sArray[2]);

        // store as a Vector3
        Vector3 result = new Vector3(
            xCoordinate,
            yCoordinate,
            zCoordinate);

        return result;
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