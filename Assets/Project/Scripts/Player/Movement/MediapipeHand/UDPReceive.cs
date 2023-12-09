using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour //udp socket for mediapipe in python
{
    [Header("Socket port")]
    public int port = 25001;

    [Header("Listen on socket")]
    public bool startReceiving = true;

    [Header("Sent data")]
    public string data;

    Thread receiveThread;
    UdpClient client;

    public void Awake() //start receiving on awake
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log(gameObject.name);
    }

    public void OnDestroy() //close client on destroy
    {
        client.Close();
    }

    private void ReceiveData() //get mediapipe data from socket
    {
        client = new UdpClient(port);
        while (startReceiving)
        {
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = client.Receive(ref anyIP);
            string text = Encoding.UTF8.GetString(data);
            this.data = text;
        }
    }
}