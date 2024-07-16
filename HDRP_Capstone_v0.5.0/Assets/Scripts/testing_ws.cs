using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket websocket;

    void Start()
    {
        // Create a new WebSocket object with the URL of your WebSocket server
        websocket = new WebSocket("ws://localhost:8080/?client=Unity");

        // Subscribe to the Message event to handle incoming messages
        websocket.OnMessage += (sender, e) =>
        {
            if (e.IsText)
            {
                Debug.Log("Text message received from server is text");
                Debug.Log("Text message received from server: " + e.Data);
            }
            else if (e.IsBinary)
            {
                // ���̳ʸ� �����͸� UTF-8 ���ڿ��� ��ȯ
                string message = System.Text.Encoding.UTF8.GetString(e.RawData);
                Debug.Log("Text message received from server is RawData");
                Debug.Log("Binary message converted to string: " + message);
            }
        };

        // Connect to the WebSocket server
        websocket.Connect();
    }

    void OnDestroy()
    {
        // Ensure the WebSocket is properly closed when the GameObject is destroyed
        if (websocket != null)
        {
            websocket.Close();
            websocket = null; 
        }
    }
}

