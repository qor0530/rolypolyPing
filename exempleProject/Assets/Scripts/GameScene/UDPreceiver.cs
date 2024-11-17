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
        catch (Exception e)
        {
            Debug.LogError($"UDP 수신 중 오류 발생: {e.Message}");
        }
    }

    private void ParseJsonData(string jsonData)
    {
        try
        {
            // JSON 파싱
            var parsedData = JsonUtility.FromJson<Wrapper>(jsonData);


            // 데이터 처리
            foreach (var user in parsedData.data)
            {
                Debug.Log(user.score);
                Debug.Log(user.coord_3d);


            }

        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 파싱 중 오류 발생: {e.Message}");
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