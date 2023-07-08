using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;

public class HandScriptRotation : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;

    Vector3[] mpPoints;

    Vector3 handPosition;

    private Transform[] handJoints;
    public GameObject Hand;

    public static Vector3 minPoint;
    public static Vector3 maxPoint;

    private float cameraHeight;
    private float cameraWidth;
    public static float z;

    bool running;

    private void Update() // 1. Get all points as an array or vectors 2. Calculate the rotation in joints on 2 different planes 3. Rotate hand using local variables || IN THIS ORDER 
    {
        if (mpPoints != null && mpPoints.Length > 0)
        {
            RotateJoints();
        }
        Vector3 newPos = RotateAroundPoint(handPosition, PlayerParams.Objects.playerCamera.transform.position, PlayerParams.Objects.playerCamera.transform.eulerAngles.y);
        transform.position = newPos; //assigning receivedPos in SendAndReceiveData()
    }

    private void Start()
    {
        CalculateNearPlaneBounds();

        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    private void CalculateNearPlaneBounds()
    {
        Hand = GameObject.Find("Armature");
        handJoints = Hand.transform.GetComponentsInChildren<Transform>();
        handJoints = Array.FindAll(handJoints, t => t != Hand.transform);

        foreach (Transform joint in handJoints)
        {
            Debug.Log(joint.name);
        }

        Vector3 objectPosition = transform.position;
        Vector3 objectPositionInCameraSpace = PlayerParams.Objects.playerCamera.transform.InverseTransformPoint(objectPosition);
        float distance = Mathf.Abs(objectPositionInCameraSpace.z);
        float halfHeight = distance * Mathf.Tan(PlayerParams.Objects.playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * PlayerParams.Objects.playerCamera.aspect;

        float minX = -halfWidth;
        float maxX = halfWidth;
        float minY = -halfHeight;
        float maxY = halfHeight;
        z = objectPosition.z;


        minPoint = PlayerParams.Objects.playerCamera.transform.TransformPoint(new Vector3(minX, minY, distance));
        maxPoint = PlayerParams.Objects.playerCamera.transform.TransformPoint(new Vector3(maxX, maxY, distance));

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

    private void RotateJoints()
    {// [6] i [21]
        Transform WholeHand = handJoints[0];
        Vector3 rotation = WholeHand.localRotation.eulerAngles;
        rotation.x = CalculateAngleOfRotationX(mpPoints[9], mpPoints[0]);
        rotation.y = CalculateAngleOfRotationY(mpPoints[9], mpPoints[13]);
        //rotation.z = CalculateAngleOfRotationZ(mpPoints[0], mpPoints[9]);
        WholeHand.localRotation = Quaternion.Euler(rotation);

        Transform IndexFinger1 = handJoints[6];
        Vector3 rotation2 = IndexFinger1.localRotation.eulerAngles;
        //rotation2.x = CalculateAngleOfRotationX(mpPoints[7], mpPoints[6]) - rotation.x;
        //rotation2.z = -(CalculateAngleOfRotationZ(mpPoints[6], mpPoints[5]) + 180);
        IndexFinger1.localRotation = Quaternion.Euler(rotation2);
    }

    public float CalculateAngleOfRotationX(Vector3 A, Vector3 B)
    {
        Debug.Log(A);

        Debug.Log(B);

        Vector3 direction = A - B;

        float theta = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;

        return -theta;
    }


    public float CalculateAngleOfRotationY(Vector3 A, Vector3 B)
    {
        Vector2 positionA = new Vector2(A.x, A.y);
        Vector2 positionB = new Vector2(B.x, B.y);
        Vector2 positionC = new Vector2(B.x + 10, B.y);

        Vector2 direction1 = positionB - positionA;
        Vector2 direction2 = positionC - positionB;

        float cosTheta = Vector2.Dot(direction1, direction2) / (direction1.magnitude * direction2.magnitude);
        float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

        if (positionA.x >= positionB.x && positionA.y >= positionB.y)
        {
            return -theta;
        }
        else if (positionA.x >= positionB.x && positionA.y <= positionB.y)
        {
            return theta;
        }
        else if (positionA.x <= positionB.x && positionA.y >= positionB.y)
        {
            return -theta;
        }
        else if (positionA.x <= positionB.x && positionA.y <= positionB.y)
        {
            return theta;
        } else
        {
            return 0f;
        }
    }


    public float CalculateAngleOfRotationZ(Vector3 A, Vector3 B)
    {
        Vector2 positionA = new Vector2(A.x, A.z);
        Vector2 positionB = new Vector2(B.x, B.z);
        Vector2 positionC = new Vector2(B.x + 10, B.z);

        Vector2 direction1 = positionB - positionA;
        Vector2 direction2 = positionC - positionB;

        float cosTheta = Vector2.Dot(direction1, direction2) / (direction1.magnitude * direction2.magnitude);
        float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

        if (positionA.x >= positionB.x && positionA.y >= positionB.y)
        {
            return -theta - 90;
        }
        else if (positionA.x >= positionB.x && positionA.y <= positionB.y)
        {
            return theta - 90;
        }
        else if (positionA.x <= positionB.x && positionA.y >= positionB.y)
        {
            return -theta - 90;
        }
        else if (positionA.x <= positionB.x && positionA.y <= positionB.y)
        {
            return theta - 90;
        }
        else
        {
            return 0f;
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
            handPosition = mpPoints[0];
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
                float zAxis = z - float.Parse(coordinates[2]);

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
