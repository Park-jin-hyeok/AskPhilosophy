using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject Client;
    public int sceneChangeNum = 0;
    public RawImage imageToFade; // Assign this in the inspector
    public float fadeDuration = 1.0f; // Duration of the fade

    private void Start()
    {
        StartCoroutine(FadeImageFromFullToZero());
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                StartCoroutine(FadeImageAndChangeScene());
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

    IEnumerator FadeImageAndChangeScene()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            Color newColor = new Color(0, 0, 0, alpha);
            imageToFade.color = newColor;
            yield return null;
        }

        SceneManager.LoadScene(1);

        if (sceneChangeNum > 0)
        {
            Client.GetComponent<Client>().ws.ConnectAsync();
        }
    }
}
