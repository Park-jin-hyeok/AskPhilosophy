using System.Collections;
using UnityEngine;
using WebSocketSharp;

public class Client : MonoBehaviour
{
    public WebSocket ws;

    private void Start()
    {
        // 'ws://example.com'은 연결하고자 하는 웹소켓 서버의 주소와 포트로 교체해야 합니다.
        ws = new WebSocket("ws://52.79.189.240:8080");

        ws.OnOpen += OnOpen;
        ws.OnMessage += OnMessageReceived;
        ws.OnError += OnError;
        ws.OnClose += OnClose;

        ws.Connect();
    }

    private void Update()
    {
        // If the space key is pressed and the WebSocket connection is open, send the message.
        if (Input.GetKeyDown(KeyCode.Space) && ws.ReadyState == WebSocketState.Open)
        {
            SendMessageToServer("...................");
        }
        if (Input.GetKeyDown(KeyCode.X) && ws.ReadyState == WebSocketState.Open)
        {
            SendMessageToServer("2");
        }
        if (Input.GetKeyDown(KeyCode.C) && ws.ReadyState == WebSocketState.Open)
        {
            SendMessageToServer("0");
        }
        if (Input.GetKeyDown(KeyCode.V) && ws.ReadyState == WebSocketState.Open)
        {
            SendMessageToServer("1");
        }
    }

    public void SendMessageToServer(string message)
    {
        ws.Send(message);
        Debug.Log("Sent message: " + message);
    }

    private void OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connection opened.");
        // 여기서 서버로 메시지를 보낼 수 있습니다.
        ws.Send("Hello, Server!");
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        Debug.Log("Message received from server: " + e.Data);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("Error occurred: " + e.Message);
    }

    private void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket connection closed.");
    }

    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using WebSocketSharp;

//[Flags]
//public enum SslProtocols
//{
//    None = 0,
//    Ssl2 = 12,
//    Ssl3 = 48,
//    Tls = 192,
//    Default = 240,
//    Tls11 = 768,
//    Tls12 = 3072
//}

//public class Client : MonoBehaviour
//{
//    public string serverUrl = "ws://52.79.189.240:8080";
//    public WebSocket ws;

//    // Define the key-message pairs
//    [System.Serializable]
//    public struct KeyMessagePair
//    {
//        public KeyCode key;
//        public string message;
//    }

//    public KeyMessagePair[] keyMessagePairs;

//    void Start()
//    {
//        ws = new WebSocket(serverUrl);
//        // ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

//        ws.OnOpen += OnOpenHandler;
//        ws.OnClose += OnCloseHandler;
//        ws.Connect();
//    }

//    void Update()
//    {
//        foreach (var pair in keyMessagePairs)
//        {
//            if (Input.GetKeyDown(pair.key))
//            {
//                SendMessage(pair.message);
//            }
//        }
//    }

//    public void SendMessage(string message)
//    {
//        if (ws.ReadyState == WebSocketState.Open)
//        {
//            ws.Send(message);
//            Debug.Log("Sent message: " + message);
//        }
//        else
//        {
//            Debug.LogWarning("WebSocket is not connected.");
//        }
//    }

//    void OnOpenHandler(object sender, System.EventArgs e)
//    {
//        Debug.Log("WebSocket connected!");
//    }

//    void OnCloseHandler(object sender, CloseEventArgs e)
//    {
//        Debug.Log("WebSocket closed with reason: " + e.Reason);
//    }

//    void OnDestroy()
//    {
//        if (ws != null)
//        {
//            ws.CloseAsync();
//        }
//    }
//}
