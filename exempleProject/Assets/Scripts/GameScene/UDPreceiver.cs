using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    public int port = 25712;


    void Start()
    {
        // UDP Ŭ���̾�Ʈ �ʱ�ȭ
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"UDP ������ {port} ��Ʈ���� ��� ���Դϴ�.");
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

        try
        {
            while (true)
            {
                // ������ ����
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log($"���� ������: {message}");

                // JSON �Ľ�
                ParseJsonData(message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"UDP ���� �� ���� �߻�: {e.Message}");
        }
    }

    private void ParseJsonData(string jsonData)
    {
        try
        {
            // JSON �Ľ�
            var parsedData = JsonUtility.FromJson<Wrapper>(jsonData);


            // ������ ó��
            foreach (var user in parsedData.data)
            {
                Debug.Log(user.score);
                Debug.Log(user.coord_3d);


            }

        }
        catch (Exception e)
        {
            Debug.LogError($"JSON �Ľ� �� ���� �߻�: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }

    [Serializable]
    public class Wrapper
    {
        public List<User> data;
    }

    [Serializable]
    public class User
    {
        public float score;
        public List<List<float>> coord_3d;
    }
}