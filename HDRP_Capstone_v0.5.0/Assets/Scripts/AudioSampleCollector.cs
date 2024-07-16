using UnityEngine;
using System;
using System.IO;

[RequireComponent(typeof(AudioSource))]
public class AudioSampleCollector : MonoBehaviour
{
    public int sampleDataLength = 800; // Length of the sample data
    private float[] _audioData; // Buffer for audio data
    private AudioSource _audioSource; // AudioSource component

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = Microphone.Start(null, true, 1, 44100); // Start recording from the microphone
        _audioSource.loop = true; // Loop the recording
        _audioSource.mute = true; // Mute the playback
        while (!(Microphone.GetPosition(null) > 0)) { } // Wait until the recording has started
        _audioSource.Play(); // Start playing the audio source

        _audioData = new float[sampleDataLength];
    }

    /*
    public float[] GetRecentAudioWindow()
    {
        if (_audioSource.clip == null) return null;

        int micPosition = Microphone.GetPosition(null) - sampleDataLength;
        if (micPosition < 0) return null;

        _audioSource.clip.GetData(_audioData, micPosition);
        return _audioData;
    }
    */

    public float[] GetRecentAudioWindow()
    {
        if (_audioSource.clip == null) return null;

        int micPosition = Microphone.GetPosition(null) - sampleDataLength;
        if (micPosition < 0) return null;

        _audioSource.clip.GetData(_audioData, micPosition);

        // ���⿡�� ����� �������� ��� ������ ����մϴ�.
        float averageVolume = CalculateAverageVolume(_audioData);

        // �Ӱ谪�� �����մϴ�. �� ���� �����Ͽ� �ΰ����� ������ �� �ֽ��ϴ�.
        float threshold = 0.01f;

        // ��� ������ �Ӱ谪 ������ ���, �����մϴ�.
        if (averageVolume < threshold)
        {
            return null;
        }

        return _audioData;
    }

    // ����� �����Ϳ��� ��� ������ ����ϴ� �޼����Դϴ�.
    private float CalculateAverageVolume(float[] audioData)
    {
        float sum = 0;

        foreach (var sample in audioData)
        {
            sum += Math.Abs(sample); // ������ ���밪�� ����մϴ�.
        }

        return sum / audioData.Length;
    }


    public float[] ReadAudioFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        AudioClip clip = null; // Load your audio file into this AudioClip

        // Here, you would implement the code to load an audio file into the 'clip'.
        // This is platform and format dependent and may require additional libraries.

        if (clip == null) return null;

        float[] audioData = new float[sampleDataLength];
        if (clip.samples < sampleDataLength) return null;

        clip.GetData(audioData, 0);
        return audioData;
    }
}
