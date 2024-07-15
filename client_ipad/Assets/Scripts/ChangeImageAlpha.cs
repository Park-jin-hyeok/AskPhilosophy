using UnityEngine;
using UnityEngine.UI;

public class ChangeImageAlpha : MonoBehaviour
{
    public KeyCode increaseAlphaKey = KeyCode.C;
    public KeyCode decreaseAlphaKey = KeyCode.V;
    public KeyCode BlackAlphaKey = KeyCode.I;

    private float duration = 3.0f;

    private RawImage rawImage;
    private Color color;
    private float targetAlpha;
    private float alphaVelocity = 0.0f;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        color = rawImage.color;
    }

    void Update()
    {
        color.a = Mathf.SmoothDamp(color.a, targetAlpha, ref alphaVelocity, duration);
        rawImage.color = color;

        if (Input.GetKeyDown(increaseAlphaKey))
        {
            duration = 0f;
            targetAlpha = 1.0f;
        }
        else if (Input.GetKeyDown(decreaseAlphaKey))
        {
            duration = 1f;
            targetAlpha = 0.0f;
        }
    }
}