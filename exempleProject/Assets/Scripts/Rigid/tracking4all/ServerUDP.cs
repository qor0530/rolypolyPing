using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Net;
using System.Threading;

/// <summary>
/// A basic UDP server that listens to a port for incoming data.
/// </summary>
class ServerUDP
{
    UdpClient server;
    IPEndPoint endPoint;
    bool open;

    const int BUFFER_SIZE = 16384;
    byte[] buffer = new byte[BUFFER_SIZE];
    string latestResponse;

    Queue<string> messageBuffer = new Queue<string>(); // Used when receive messages faster than we can process it.
    int maxMessageBufferSize;
    bool suppressWarnings;

    int port;
    byte[] aliveBuffer = new byte[] { 1 };

    public ServerUDP(string ip, int port, bool suppressWarnings = true, int maximumMessageBufferSize = 100)
    {
        server = new UdpClient(port, AddressFamily.InterNetwork);
        endPoint = default(IPEndPoint);
        maxMessageBufferSize = maximumMessageBufferSize;
        this.port = port;
        this.suppressWarnings = suppressWarnings;
    }
    public void Connect()
    {
        open = true;
    }
    public void Disconnect()
    {
        server.Close();
        open = false;
    }
    public void StartListeningAsync()
    {
        Thread t = new Thread(new ThreadStart(StartListening));
        t.Start();
    }
    public void StartListening()
    {
        print("Waiting for messages @Port:" + port);
        messageBuffer.Clear();
        open = true;

        while (open)
        {
            try
            {
                buffer = server.Receive(ref endPoint);
                if (buffer.Length > 0)
                {
                    latestResponse = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                    // 이전에는 <EOM>으로 메시지를 구분했지만 이제는 하나의 패킷이 하나의 메시지라고 가정
                    if (messageBuffer.Count >= maxMessageBufferSize)
                    {
                        messageBuffer.Dequeue();
                        if (!suppressWarnings)
                            print("Too slow to keep up with packets being sent (dropping old data). Make sure you are getting messages with GetMessage().");
                    }

                    messageBuffer.Enqueue(latestResponse);
                    print("(" + messageBuffer.Count + ") " + latestResponse);
                }
            }
            catch (SocketException ex)
            {
                print("Connection lost.");
                Thread.Sleep(1000);
                StartListening();
                break;
            }
        }
    }

    public bool HasMessage()
    {
        return messageBuffer.Count > 0;
    }
    public string GetMessage()
    {
        return messageBuffer.Dequeue();
    }

    private void print(object o)
    {
        Console.WriteLine(o);
    }
}
