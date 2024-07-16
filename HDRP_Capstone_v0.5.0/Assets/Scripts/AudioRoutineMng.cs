using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class AudioRoutineMng : MonoBehaviour
{
    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isAudioPlaying = false;

    private string clientId = "";
    private string clientSecret = "";

    string filePath = "D:/repos/temp/temp.mp3";
    public AudioSource audioSource;

    public GameObject chatman;
    private ChatManager chatMngComp;

    public void AddtoDialogueQueue(List<string> dialogueList)
    {

        QueueListOfDialogue(dialogueList);
        chatMngComp = chatman.GetComponent<ChatManager>();

    }

    private void QueueListOfDialogue(List<string> dialogueList)
    {
        foreach (string dialogue in dialogueList)
        {
            Debug.Log($"added {dialogue}");
            dialogueQueue.Enqueue(dialogue);
        }

        if (!isAudioPlaying)
        {
            StartCoroutine(ProcessNextDialogue());
        }
    }

    private IEnumerator ProcessNextDialogue()
    {
        Debug.Log($"1");
        if (dialogueQueue.Count > 0 && !isAudioPlaying)
        {
            Debug.Log($"1");
            yield return SendTTSGenerationRequestToClova();
        }
    }

    private IEnumerator SendTTSGenerationRequestToClova()
    {
        Debug.Log($"1");
        if (dialogueQueue.Count == 0)
        {
            yield break; // Queue is empty, end coroutine
        }

        Debug.Log("??");
        string dialogue = dialogueQueue.Dequeue();
        Debug.Log(dialogue);
        Task task = GenerateAudioFromTTS(dialogue);

        Debug.Log("Wait till finished");
        yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log("Finished generating TTS");

        PlayAudio();
    }

    private int GetStatueIdxfromChatMng()
    {
        return chatMngComp.getCurrentStateIdx();
    }

    private async Task GenerateAudioFromTTS(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            int statueIdx = GetStatueIdxfromChatMng();
            var content = new StringContent($"speaker=nwontak&volume=0&speed=0&pitch={statueIdx * 2}&format=mp3&text={text}",
                                            Encoding.UTF8,
                                            "application/x-www-form-urlencoded");

            // Add headers
            client.DefaultRequestHeaders.Add("X-NCP-APIGW-API-KEY-ID", clientId);
            client.DefaultRequestHeaders.Add("X-NCP-APIGW-API-KEY", clientSecret);

            // Send a POST request
            Debug.Log("TTS generation");
            HttpResponseMessage response = await client.PostAsync("https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts", content);

            if (response.IsSuccessStatusCode)
            {
                Debug.Log("TTS mp3 file downloaded");
                using (Stream input = await response.Content.ReadAsStreamAsync())
                using (FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await input.CopyToAsync(output);
                }
                Console.WriteLine(filePath +" was created");

                // Here, you can add your logic to load the audio file into an AudioClip and play it
                // This part of the code depends on how you handle audio in your application
                // For example, using Unity's audio system or another library
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }

    public void PlayAudio()
    {
        Debug.Log("Play audio from path task inited");
        StartCoroutine(PlayAudioFromPath(filePath));
    }

    //public void PlayAudio(AudioClip clip)
    //{
    //    AudioSource audioSource = GetComponent<AudioSource>();
    //    if (audioSource != null && clip != null)
    //    {
    //        audioSource.clip = clip;
    //        audioSource.Play();
    //        isAudioPlaying = true;
    //        StartCoroutine(WaitForAudioEnd(audioSource));
    //    }
    //}

    IEnumerator PlayAudioFromPath(string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            Debug.Log("Audio Retrival complete");
            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log("does there exist a clip?");
                if (clip != null)
                {
                    Debug.Log("Yse");
                    audioSource.clip = clip;

                    audioSource.Play();
                    Debug.Log("Audio played successfully.");

                    isAudioPlaying = true;
                    StartCoroutine(WaitForAudioEnd(audioSource));
                }
                else
                {
                    Debug.Log("no");
                }
            }
            else
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
        }
    }
    private IEnumerator WaitForAudioEnd(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        chatMngComp.PhilosopherDequeueAndToggleLipSync();
        isAudioPlaying = false;
        StartCoroutine(ProcessNextDialogue()); // Process next dialogue after audio finishes
    }
}
