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

    // Singleton 인스턴스
    public static UDPReceiver Instance { get; private set; }

    // 저장된 데이터를 외부에서 접근 가능하도록
    public List<List<float>> LatestCoord3D { get; private set; } = new List<List<float>>();

    void Awake()
    {
        // Singleton 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 다른 씬으로 넘어가도 유지
        }
        else
        {
            Destroy(gameObject); // Singleton 중복 방지
        }
    }

    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"UDP 서버가 {port} 포트에서 대기 중입니다.");
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
                Debug.Log($"수신 데이터: {message}");

                ParseJsonData(message);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UDP 수신 중 오류 발생: {e.Message}");
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
                    // 최신 데이터를 저장
                    LatestCoord3D = user.coord_3d;
                    Debug.Log("데이터가 업데이트되었습니다.");
                }
                else
                {
                    Debug.LogWarning("coord_3d 데이터가 부족하거나 null입니다.");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 파싱 중 오류 발생: {e.Message}");
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
        public List<List<float>> coord_3d; // 2D 리스트
    }
}
