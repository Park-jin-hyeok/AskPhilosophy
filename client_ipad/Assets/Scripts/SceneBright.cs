using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneBright : MonoBehaviour
{
    public RawImage imageToFade; // Assign this in the inspector
    public float fadeDuration = 1.0f; // Duration of the fade

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeImageFromFullToZero());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            // This will capture most alphanumeric keys and print them
            string keysPressed = Input.inputString;
            if (!string.IsNullOrEmpty(keysPressed))
            {
                Debug.Log("Key(s) pressed: " + keysPressed);
            }
        }
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                // This will get the name of the key and append it to the text area
                Debug.Log(keyCode + " pressed\n");
            }
        }
    }
    IEnumerator FadeImageFromFullToZero()
    {
        float elapsedTime = 0.0f;
        Color startColor = imageToFade.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1.0f - elapsedTime / fadeDuration);
            imageToFade.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
    }
}
