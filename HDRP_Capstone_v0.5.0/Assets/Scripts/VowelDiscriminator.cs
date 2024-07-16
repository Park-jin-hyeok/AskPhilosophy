using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class VowelDiscriminator : MonoBehaviour 
    // Notifier of the current vowel 
    // other classes that needs the current vowel are expected to look at this class
{
    public NNModel modelAsset;
    public AudioSource audioSource; // AudioSource component for audio input
    public int audioSampleUnitLength = 800; // Length of audio input
    public float audioThreshold = 0.001f;

    public string dominantVowel = "a";
    public float magnitude = 0;

    private Model _runtimeModel;
    private IWorker _worker;

    // Start is called before the first frame update
    void Start()
    {
        // Set the audioSource to GoogleTranslateTTS's audioSource
        audioSource = GoogleTranslateTTS.Instance.audioSource;

        // Load the model
        _runtimeModel = ModelLoader.Load(modelAsset);
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, _runtimeModel);
    }

    // Update is called once per frame
    void Update()
    {
        float loudness = CalculateLoudness();
        // set magnitude 
        magnitude = loudness; 

        if (loudness < audioThreshold)
        {
            // when no audio don't run the model
            return;
        }
        else
        {
            // Process audio data from AudioSource
            ProcessAudioData();
        }
    }

    float CalculateLoudness()
    {
        // Ensure the audio source has a clip
        if (audioSource == null || audioSource.clip == null)
        {
            return 0f;
        }

        // Calculate start index for the most recent audio data
        int startIndex = Mathf.Max(0, audioSource.timeSamples - audioSampleUnitLength);

        // Ensure the startIndex does not exceed the bounds of the clip data
        startIndex = Mathf.Min(startIndex, audioSource.clip.samples - audioSampleUnitLength);

        // Get the most recent audio data
        float[] audioData = new float[audioSampleUnitLength];
        audioSource.clip.GetData(audioData, startIndex);

        // Calculate RMS value
        float sum = 0;
        for (int i = 0; i < audioData.Length; i++)
        {
            sum += audioData[i] * audioData[i]; // sum of squares
        }

        float rmsValue = Mathf.Sqrt(sum / audioData.Length); // root mean square

        // Convert to decibels (optional)
        // float dBValue = 20 * Mathf.Log10(rmsValue / referenceValue); // referenceValue is usually 1.0

        return rmsValue; // Or return dBValue if using decibels
    }

    //void ProcessAudioData()
    //{
    //    // Get audio data from AudioSource
    //    float[] audioData = new float[audioSampleUnitLength];
    //    audioSource.clip.GetData(audioData, 0);

    //    // Preprocess the audio data (if necessary)
    //    var preprocessedData = PreprocessData(audioData);

    //    // Create a tensor from the preprocessed data
    //    var input = new Tensor(1, 1, 1, audioSampleUnitLength, preprocessedData);

    //    // Execute the model
    //    _worker.Execute(input);
    //    var output = _worker.PeekOutput();

    //    // Interpret the output
    //    string detectedVowel = InterpretOutput(output);
    //    Debug.Log("Detected Vowel: " + detectedVowel);

    //    // set dominant vowel 
    //    dominantVowel = detectedVowel; 

    //    // Cleanup
    //    input.Dispose();
    //    output.Dispose();
    //}

    void ProcessAudioData()
    {
        // Calculate start index for the most recent audio data
        int startIndex = Mathf.Max(0, audioSource.timeSamples - audioSampleUnitLength);

        // Ensure the startIndex does not exceed the bounds of the clip data
        startIndex = Mathf.Min(startIndex, audioSource.clip.samples - audioSampleUnitLength);

        // Get the most recent audio data
        float[] audioData = new float[audioSampleUnitLength];
        audioSource.clip.GetData(audioData, startIndex);

        // Preprocess the audio data (if necessary)
        var preprocessedData = PreprocessData(audioData);

        // Create a tensor from the preprocessed data
        var input = new Tensor(1, 1, 1, audioSampleUnitLength, preprocessedData);

        // Execute the model
        _worker.Execute(input);
        var output = _worker.PeekOutput();

        // Interpret the output
        string detectedVowel = InterpretOutput(output);
        Debug.Log("Detected Vowel: " + detectedVowel);

        // set dominant vowel 
        dominantVowel = detectedVowel;

        // Cleanup
        input.Dispose();
        output.Dispose();
    }

    private float[] PreprocessData(float[] data)
    {
        // Implement necessary preprocessing
        return data; // Placeholder, replace with actual preprocessing
    }

    private string InterpretOutput(Tensor output)
    {
        // Assuming the output is a 1-D tensor of probabilities, one for each vowel class
        var probabilities = output.ToReadOnlyArray();

        // Find the index of the maximum probability, which corresponds to the most likely class
        int maxIndex = 0;
        float maxProbability = probabilities[0];
        for (int i = 1; i < probabilities.Length; i++)
        {
            if (probabilities[i] > maxProbability)
            {
                maxProbability = probabilities[i];
                maxIndex = i;
            }
        }

        // Array of vowel classes corresponding to the model's output
        string[] vowelClasses = { "a", "e", "i", "o", "u" };

        // Check if the maxIndex is within the bounds of the vowelClasses array
        if (maxIndex >= 0 && maxIndex < vowelClasses.Length)
        {
            return vowelClasses[maxIndex];
        }
        else
        {
            Debug.LogError("Model output index out of range.");
            return "";
        }
    }

    void OnDestroy()
    {
        // Clean up the worker when the script is destroyed
        _worker.Dispose();
    }
}