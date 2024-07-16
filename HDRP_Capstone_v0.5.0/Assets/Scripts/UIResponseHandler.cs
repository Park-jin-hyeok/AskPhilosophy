using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIResponseHandler : MonoBehaviour
{
    public ChatManager chatManager;
    public TextMeshProUGUI responseText; // Reference to your TMP Text UI element

    void Start()
    {
        if (chatManager != null)
        {
            chatManager.OnResponse.AddListener(UpdateResponseText);
        }
    }

    void UpdateResponseText(string message)
    {
        responseText.text = message;
    }
}