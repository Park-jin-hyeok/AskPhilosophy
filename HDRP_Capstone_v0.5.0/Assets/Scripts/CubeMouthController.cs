using UnityEngine;

public class CubeMouthController : MonoBehaviour
{
    public AudioSource audioSource;
    public bool lipSyncToggle = false;
    // Frequency data from audio
    private float[] spectrum = new float[256];

    void Start()
    {
        // Set the color to red
        GetComponent<Renderer>().material.color = Color.red;

        // Set the audioSource to GoogleTranslateTTS's audioSource
        audioSource = GoogleTranslateTTS.Instance.audioSource;
    }

    void Update()
    {
        if (lipSyncToggle)
        {
            // Get spectrum data from audio
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

            // Compute average to represent "loudness" or complexity of the audio
            float average = 0;
            for (int i = 0; i < spectrum.Length; i++)
            {
                average += spectrum[i];
            }
            average /= spectrum.Length;

            // Scale the cube (mouth) based on loudness (adjust scaling factor as needed)
            float scaleFactor = average * 100f;
            transform.localScale = new Vector3(0.4f, scaleFactor, 0.2f);
        }
    }
}
