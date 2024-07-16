using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class Client : MonoBehaviour
{
    public WebSocket ws;
    public GameObject chatMng;
    NoddingAnim animAristotle;
    NoddingAnim animSeneka;

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
        //// If the space key is pressed and the WebSocket connection is open, send the message.
        //if (Input.GetKeyDown(KeyCode.Space) && ws.ReadyState == WebSocketState.Open)
        //{
        //    SendMessageToServer("...................");
        //}
    }

    //private void SendMessageToServer(string message)
    //{
    //    ws.Send(message);
    //    Debug.Log("Sent message: " + message);
    //}

    private void OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connection opened.");
        // 여기서 서버로 메시지를 보낼 수 있습니다.
        ws.Send("Hello, Server!");
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        if (e.IsText)
        {
            // If the message is text, use e.Data
            Debug.Log("Message received from server: " + e.Data);
            if (e.Data == "0")
            {
                Debug.Log("Start command received. Triggering exhibition logic.");
                JointController.b = 1;
            }
            else if (e.Data == "1")
            {
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
            }
        }
        else if (e.IsBinary)
        {
            // If the message is binary, convert it to a string
            string message = System.Text.Encoding.UTF8.GetString(e.RawData);
            Debug.Log("Message received from server: " + message);
            if (message == "0")
            {
                Debug.Log("Start command received. Triggering exhibition logic.");
                JointController.b = 1;
            }
            else if (message == "1")
            {
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
            }
            //if (message == "2")
            //{
            //    animAristotle.GetComponent<NoddingAnim>().AristoSleeping(true);
            //    animSeneka.GetComponent<NoddingAnim>().SenekaSleeping(true);
            //}
        }

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
