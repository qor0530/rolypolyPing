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

    // Singleton �ν��Ͻ�
    public static UDPReceiver Instance { get; private set; }

    // ����� �����͸� �ܺο��� ���� �����ϵ���
    public List<List<float>> LatestCoord3D { get; private set; } = new List<List<float>>();

    void Awake()
    {
        // Singleton �ʱ�ȭ
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �ٸ� ������ �Ѿ�� ����
        }
        else
        {
            Destroy(gameObject); // Singleton �ߺ� ����
        }
    }

    void Start()
    {
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
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log($"���� ������: {message}");

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
            var parsedData = JsonConvert.DeserializeObject<Wrapper>(jsonData);

            foreach (var user in parsedData.data)
            {
                if (user.coord_3d != null && user.coord_3d.Count == 33)
                {
                    // �ֽ� �����͸� ����
                    LatestCoord3D = user.coord_3d;
                    Debug.Log("�����Ͱ� ������Ʈ�Ǿ����ϴ�.");
                }
                else
                {
                    Debug.LogWarning("coord_3d �����Ͱ� �����ϰų� null�Դϴ�.");
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
