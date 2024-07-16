using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WebSocketSharp;
using WebSocketSharp.Server;

public class Server : MonoBehaviour
{
    private WebSocketServer wss;

    void Start()
    {
        // Start WebSocket server on port 8080
        wss = new WebSocketServer("ws://0.0.0.0:8080");
        wss.AddWebSocketService<UnityWebSocketBehavior>("/Unity");
        wss.Start();

        Debug.Log("WebSocket Server started and listening on all network interfaces on port 8080");
    }

    void Update() {
    }

    void OnDestroy()
    {
        // Close the WebSocket server when the Unity object is destroyed
        if (wss != null)
        {
            wss.Stop();
            Debug.Log("WebSocket Server stopped");
        }
    }
}

public class UnityWebSocketBehavior : WebSocketBehavior
{
    GameObject ChatManager;
    GameObject SpeechToText;

    protected override void OnMessage(MessageEventArgs e) {
        // Handle message received from the client
        Debug.Log("Message Received from Client: " + e.Data);

        // You can add your custom logic here to trigger events/actions in your Unity project
        // For example, if the message is "start", you can start your exhibition logic
        if (e.Data == "0") {
            Debug.Log("Start command received. Triggering exhibition logic.");
            JointController.b = 1;
            ChatManager.GetComponent<ChatManager>();
            SpeechToText.GetComponent<SpeechToText>().num = 0;
        }
        else if (e.Data == "1") {
            Debug.Log("Start command received. Triggering exhibition logic.");

            if (JointController.c == 1)
            {
                Debug.Log("before: c 1 to c 0" + JointController.c);
                JointController.c = 0;
                Debug.Log("c 1 to c 0" + JointController.c);
            }
            else
            {
                Debug.Log("before: c 1 to c 0" + JointController.c);
                JointController.c = 1;
                Debug.Log("c 1 to c 0" + JointController.c);
            }

            ChatManager.GetComponent<ChatManager>();
            SpeechToText.GetComponent<SpeechToText>().num = 1;
        }
        //else if (e.Data == "2") {
        //    Debug.Log("Start command received. Triggering exhibition logic.");
        //    JointController.b = 1;
        //    ChatManager.GetComponent<ChatManager>().;
        //    SpeechToText.GetComponent<SpeechToText>().num = -1;
        //}
    }
}

