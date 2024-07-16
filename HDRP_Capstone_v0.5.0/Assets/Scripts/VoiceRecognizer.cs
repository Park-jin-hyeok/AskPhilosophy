using UnityEngine;
using Unity.Barracuda;
using System.Linq;
/*
using System;
using System.IO;
*/

public class VoiceRecognizer : MonoBehaviour
{
    public NNModel modelAsset;
    public AudioSampleCollector audioSampleCollector;
    public VowelDisplay vowelDisplayer;

    public string[] vowelClasses = new string[] { "a", "e", "i", "o", "u" }; // Replace with your actual classes
    public int sampleDataLength = 800; // Length of the sample data expected by the model

    private Model _runtimeModel;
    private IWorker _worker;

    //
    public SkinnedMeshRenderer skinnedMeshRenderer; // SkinnedMeshRenderer�� ���� GameObject�� �����մϴ�.
    private float targetBlendShapeValue = 60.0f; // ��ǥ BlendShape ��
    private float lerpSpeed = 100.0f; // ���� ���� �ӵ�
    private float currentBlendShapeValue = 0.0f;
    private int currentNum = 0;

    // ���⼭���� 
    // ---����ũ �Է°� �������ϸ� �׳� �Ҹ� �ȵ����� ���� (�����Ϸ�)
    // ---�������� ���� (HDRP�� ��ȯ�� �̻��ϸ� �����ϱ�)
    // ---ȭ�� �����°� ���� (HDRP�� �����ؼ� ȭ�� ������ Ȯ���غ���) (������ ���� ���� �۾��غ��� ��ȸ�� �ҷ����ҵ�...)
    // ---TTS STT �ֱ� (�̰� �Ϸ�)
    // ---���� �߿��Ѱ� TTS�� �Ը�翡 �°� �ٲ���ϴµ� �̰� ��� ���� �����غ���
    // ---������ ���� open api ������ �޾ƿ��°� �ֱ�
    // ---����ũ �Է� �ȵǸ� �α� ������ �������� ����(������ ����ũ ����ȵǾ������� ����Ƽ ����)
    void MouthChanger(string message) {
        if (message == "a") {
            Mathf.Lerp(currentBlendShapeValue, targetBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(0, targetBlendShapeValue);
            Mathf.Lerp(targetBlendShapeValue, currentBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(currentNum, currentBlendShapeValue);
            currentNum = 0;
            Debug.Log("a");
        }
        else if (message == "e") {
            Mathf.Lerp(currentBlendShapeValue, targetBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(1, targetBlendShapeValue);
            Mathf.Lerp(targetBlendShapeValue, currentBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(currentNum, currentBlendShapeValue);
            currentNum = 1;
            Debug.Log("e");
        }
        else if (message == "i") {
            Mathf.Lerp(currentBlendShapeValue, targetBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(2, targetBlendShapeValue);
            Mathf.Lerp(targetBlendShapeValue, currentBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(currentNum, currentBlendShapeValue);
            currentNum = 2;
            Debug.Log("i");
        }
        else if (message == "o") {
            Mathf.Lerp(currentBlendShapeValue, targetBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(3, targetBlendShapeValue);
            Mathf.Lerp(targetBlendShapeValue, currentBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(currentNum, currentBlendShapeValue);
            currentNum = 3;
            Debug.Log("o");
        }
        else if (message == "u") {
            Mathf.Lerp(currentBlendShapeValue, targetBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(4, targetBlendShapeValue);
            Mathf.Lerp(targetBlendShapeValue, currentBlendShapeValue, lerpSpeed * Time.deltaTime);
            skinnedMeshRenderer.SetBlendShapeWeight(currentNum, currentBlendShapeValue);
            currentNum = 4;
            Debug.Log("u");
        }
        else {
            Debug.Log("�׷����� ���µ� ���׳��׿� ����");
        }
    }

    //�������
    void Start()
    {
        

        // Set up the ONNX model
        _runtimeModel = ModelLoader.Load(modelAsset);
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, _runtimeModel);

        InvokeRepeating(nameof(RepeatedlyGetDominantVowel), 1.0f, 0.1f);
    }

    void RepeatedlyGetDominantVowel()
    {
        string dominantVowel = GetDominantVowel();

        // Here, you could potentially do something with the returned 'dominantVowel',
        // like updating the UI or making game decisions.
        // For now, it just gets the value and the GetDominantVowel method
        // itself is responsible for printing it.
    }


    public string GetDominantVowelfromAudiosource()
    {
        var data = GetMicrophoneData();

        Debug.Log(data.Length);

        if (data == null || data.Length < sampleDataLength)
        {
            Debug.LogWarning("Insufficient data from microphone.");
            return "";
        }

        // Preprocess the audio data to match the model's expected input
        var preprocessedData = PreprocessData(data);

        // Create a tensor from the preprocessed data
        var input = new Tensor(1, 1, 1, sampleDataLength, preprocessedData);

        // Execute the model
        _worker.Execute(input);
        var output = _worker.PeekOutput();

        // Post-process the model output to find the dominant class
        string dominantVowel = InterpretOutput(output);

        // Print the dominant vowel to the console
        Debug.Log("Detected Vowel: " + dominantVowel); // This line prints the result.

        //
        MouthChanger(dominantVowel);
        //


        // Clean up
        input.Dispose();
        output.Dispose();

        return dominantVowel;
    }


    public string GetDominantVowel() // from microphone
    {
        var data = GetMicrophoneData();

        Debug.Log(data.Length);

        if (data == null || data.Length < sampleDataLength)
        {
            Debug.LogWarning("Insufficient data from microphone.");
            return "";
        }

        // Preprocess the audio data to match the model's expected input
        var preprocessedData = PreprocessData(data);

        // Create a tensor from the preprocessed data
        var input = new Tensor(1, 1, 1, sampleDataLength, preprocessedData);

        // Execute the model
        _worker.Execute(input);
        var output = _worker.PeekOutput();

        // Post-process the model output to find the dominant class
        string dominantVowel = InterpretOutput(output);

        // Print the dominant vowel to the console
        Debug.Log("Detected Vowel: " + dominantVowel); // This line prints the result.

        //
        MouthChanger(dominantVowel);
        //


        // Clean up
        input.Dispose();
        output.Dispose();

        return dominantVowel;
    }

    private float[] GetMicrophoneData()
    {

        float[] recentAudioData = audioSampleCollector.GetRecentAudioWindow();
        if (recentAudioData != null)
        {
            // Do something with the audio data
            // For example, you might process, analyze, or output it
            return recentAudioData;
        }

        return null;

    }

    //private float[] getaudiosourcedata()
    //{
    //    float[] currentaudiodata;

    //    return currentaudiodata;

    //}


    private float[] PreprocessData(float[] data)
    {
        // Implement the necessary preprocessing to match what your PyTorch model expects
        // This might involve steps like Fourier transformation, Mel-frequency, windowing, etc.
        // The following is a placeholder; replace it with your actual preprocessing routine

        // For example:
        // data = PerformSTFT(data);
        // data = NormalizeData(data);

        return data; // This should be the preprocessed data
    }

    private string InterpretOutput(Tensor output)
    {
        // Simplified example to interpret the output
        // You'd need to adapt this based on what your model's output actually looks like

        // Assuming the output is a 1-D tensor of probabilities, one for each class
        var probabilities = output.ToReadOnlyArray();

        var maxProbability = probabilities.Max();
        var recognizedClassIndex = System.Array.IndexOf(probabilities, maxProbability);
        
        // Prepare a string builder for the message
        System.Text.StringBuilder message = new System.Text.StringBuilder();

        message.AppendLine("Vowel Probabilities:");
        
        // Assuming you have an array of class names (vowels) that match the order of outputs in 'probabilities'
        string[] vowelClasses = new string[] { "a", "e", "i", "o", "u" };

        for (int i = 0; i < probabilities.Length; i++)
        {
            // Append each probability to the message in a new line
            message.AppendLine($"{vowelClasses[i]}: {(probabilities[i] * 100):F2}%");
        }
        
        // Append the dominant vowel information
        message.AppendLine();
        message.AppendLine($"Dominant Vowel: {vowelClasses[recognizedClassIndex]}");

        // Convert the string builder message to a string
        string displayMessage = message.ToString();

        // Log the message to the console
        Debug.Log(displayMessage);

        // Set the text in the UI component
        if (vowelDisplayer!= null)
        {
            vowelDisplayer.SetDebugText(displayMessage);
        }
        


        if (recognizedClassIndex < 0 || recognizedClassIndex >= vowelClasses.Length)
        {
            Debug.LogError("Unrecognized output.");
            return "";
        }

        return vowelClasses[recognizedClassIndex];
    }

    void OnDestroy()
    {

        // Cancel the repeated method invocation when the object is destroyed
        CancelInvoke(nameof(RepeatedlyGetDominantVowel));
    }
}