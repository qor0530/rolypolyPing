using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

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
        catch (System.Exception e)
        {
            Debug.LogError($"UDP ���� �� ���� �߻�: {e.Message}");
        }
    }

    private void ParseJsonData(string jsonData)
    {
        try
        {
            // Newtonsoft.Json ����Ͽ� �Ľ�
            var parsedData = JsonConvert.DeserializeObject<Wrapper>(jsonData);

            // ������ ó��
            foreach (var user in parsedData.data)
            {
                Debug.Log(user.score);

                if (user.coord_3d != null && user.coord_3d.Count > 0)
                {
                    Debug.Log($"coord_3d ù ��° ��: {user.coord_3d[0][0]}, {user.coord_3d[0][1]}, {user.coord_3d[0][2]}");
                }
                else
                {
                    Debug.Log("coord_3d ����Ʈ�� ��� �ְų� null�Դϴ�.");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON �Ľ� �� ���� �߻�: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }

    [System.Serializable]
    public class Wrapper
    {
        public List<User> data;
    }

    [System.Serializable]
    public class User
    {
        public float score;
        public List<List<float>> coord_3d; // 2D ����Ʈ
    }
}
