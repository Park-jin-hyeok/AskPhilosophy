using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VowelDisplay : MonoBehaviour
{
    public TMP_Text debugText; // Change this to TMP_Text for TextMeshPro

    // Start is called before the first frame update
    void Start()
    {
        // You can initialize things here if necessary
    }

    public void SetDebugText(string text)
    {
        if (debugText != null)
        {
            debugText.text = text; // Update the text displayed in the UI
        }
        else
        {
            Debug.LogWarning("Debug Text is not assigned!");
        }
    }
}