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
        // UDP 클라이언트 초기화
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
                // 데이터 수신
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log($"수신 데이터: {message}");

                // JSON 파싱
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
            // Newtonsoft.Json 사용하여 파싱
            var parsedData = JsonConvert.DeserializeObject<Wrapper>(jsonData);

            // 데이터 처리
            foreach (var user in parsedData.data)
            {
                Debug.Log(user.score);

                if (user.coord_3d != null && user.coord_3d.Count > 0)
                {
                    Debug.Log($"coord_3d 첫 번째 값: {user.coord_3d[0][0]}, {user.coord_3d[0][1]}, {user.coord_3d[0][2]}");
                }
                else
                {
                    Debug.Log("coord_3d 리스트가 비어 있거나 null입니다.");
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
