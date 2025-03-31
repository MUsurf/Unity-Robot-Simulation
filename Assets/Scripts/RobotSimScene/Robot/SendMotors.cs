using System;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class SendMotors : MonoBehaviour
{
    //TODO - manually enter IP and port
    //TODO - it might be better to just convert the powers to a text and give to this script instead of the whole file thing
    //TODO - do both things above
    public string ip = "";
    public int port = 0;
    private string txtfile = "D:\\Unity\\Games\\SURF Robot Simulation\\Assets\\Scripts\\RobotSimScene\\Real Robot\\MotorValues.txt";

    private int count;
    public string text;
    public float timeBetweenSends = 0.5f;

    public bool wantSendMotors = false;
    private IPEndPoint ep;
    private Socket sock;
    
    private bool hasSent = true;

    void Start()
    {
        ep = new IPEndPoint(IPAddress.Parse(ip), port);
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    void Update()
    {
        if(hasSent && wantSendMotors && File.Exists(txtfile))
        {
            Invoke("sendPackage", timeBetweenSends);
            hasSent = false;
        }

    }

    void sendPackage()
    {
        if(wantSendMotors && File.Exists(txtfile))
        {
            StreamReader sr = new StreamReader(txtfile);

            text = sr.ReadToEnd();

            byte [] packetdata = Encoding.ASCII.GetBytes(text);

            try
            {
                sock.SendTo(packetdata, ep);
                count++;
                Debug.Log($"Packets Sent: <color=green>{count}</color>");
            }
            catch(Exception e)
            {
                Debug.Log($"<color=red>ohno</color> {e}");
            }

            hasSent = true;
        }

        if(!hasSent)
        {
            Debug.Log("<color=red>failed to send</color>");
            hasSent = true;
        }
    }
}