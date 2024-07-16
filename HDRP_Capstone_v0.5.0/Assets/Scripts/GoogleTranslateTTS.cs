/*
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GoogleTranslateTTS : MonoBehaviour
{
    public AudioSource audioSource;

    public void Start()
    {
        // ���� �ؽ�Ʈ
        // �̰� ���� ��ȸ�� api�������ָ� �������� ��ȯ ���� (���������� �����ؾ��ҵ�?)
        string textToSpeak = "�ȳ��ϼ���";

        // Google Translate TTS URL
        string url = $"https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen={textToSpeak.Length}&client=tw-ob&q={WWW.EscapeURL(textToSpeak)}&tl=ko";

        // ����� Ŭ�� �ٿ�ε� �� ��� ����
        StartCoroutine(DownloadAndPlayAudio(url));
    }

    IEnumerator DownloadAndPlayAudio(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError($"Failed to download audio clip: {www.error}");
            }
        }
    }
}
*/

/*
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GoogleTranslateTTS : MonoBehaviour
{
    public static GoogleTranslateTTS Instance; // Singleton
    public AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ConvertAndPlay(string textToSpeak)
    {
        string url = $"https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen={textToSpeak.Length}&client=tw-ob&q={WWW.EscapeURL(textToSpeak)}&tl=ko";
        StartCoroutine(DownloadAndPlayAudio(url));
    }

    IEnumerator DownloadAndPlayAudio(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError($"Failed to download audio clip: {www.error}");
            }
        }
    }
}
*/

//using UnityEngine;
//using UnityEngine.Networking;
//using System.Collections;
//using System.Collections.Generic;
//using System;

//public class GoogleTranslateTTS : MonoBehaviour
//{
//    public static GoogleTranslateTTS Instance;
//    public AudioSource audioSource;
//    public int maxLength = 100; // Maximum length for TTS

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void ConvertAndPlay(string textToSpeak)
//    {
//        // Split the text into sentences or other logical units
//        List<string> sentences = new List<string>();

//        int startIndex = 0;
//        while (startIndex < textToSpeak.Length)
//        {
//            int sentenceLength = Math.Min(maxLength, textToSpeak.Length - startIndex);
//            int lastIndex = startIndex + sentenceLength;

//            // Try to break at the last complete word or sentence
//            if (lastIndex < textToSpeak.Length)
//            {
//                int lastSpace = textToSpeak.LastIndexOf(" ", lastIndex, StringComparison.Ordinal);
//                if (lastSpace != -1)
//                {
//                    lastIndex = lastSpace;
//                }
//            }

//            string sentence = textToSpeak.Substring(startIndex, lastIndex - startIndex).Trim();
//            sentences.Add(sentence);
//            startIndex = lastIndex;
//        }

//        // Generate TTS for each sentence
//        StartCoroutine(DownloadAndPlayAudio(sentences));
//    }

//    IEnumerator DownloadAndPlayAudio(List<string> sentences)
//    {
//        List<AudioClip> clips = new List<AudioClip>();

//        foreach (string sentence in sentences)
//        {
//            string url = GenerateUrl(sentence);
//            AudioClip clip = null;
//            Debug.Log("downloadandplay:" + sentence);

//            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
//            {
//                yield return www.SendWebRequest();

//                if (www.result == UnityWebRequest.Result.Success)
//                {
//                    Debug.Log("downloaded clip");
//                    clip = DownloadHandlerAudioClip.GetContent(www);
//                }
//                else
//                {
//                    Debug.LogError($"Failed to download audio clip: {www.error}");
//                    yield break;
//                }
//            }

//            if (clip != null)
//            {
//                clips.Add(clip);
//            }
//        }

//        // Combine all audio clips into one
//        AudioClip combinedClip = CombineAudioClips(clips);

//        // Finally play the combined audio clip
//        if (combinedClip != null)
//        {
//            audioSource.clip = combinedClip;
//            Debug.Log("Play Audio");
//            audioSource.Play();
//        }
//        else
//        {
//            Debug.Log("Combined clip is null");
//        }
//    }

//    AudioClip CombineAudioClips(List<AudioClip> clips)
//    {
//        if (clips.Count == 0) return null;

//        // Calculate the total length of the combined clip
//        int combinedLength = 0;
//        foreach (var clip in clips)
//        {
//            combinedLength += clip.samples;
//        }

//        // Create a new AudioClip
//        AudioClip combinedClip = AudioClip.Create("Combined Clip", combinedLength, 1, 44100, false);

//        // Temporary buffer
//        float[] buffer = new float[combinedLength];

//        // Current position in the buffer
//        int position = 0;

//        foreach (var clip in clips)
//        {
//            // Get data from the clip and copy it into the buffer
//            float[] data = new float[clip.samples];
//            clip.GetData(data, 0);
//            data.CopyTo(buffer, position);
//            position += clip.samples;
//        }

//        // Set the data in the combined clip
//        combinedClip.SetData(buffer, 0);

//        return combinedClip;
//    }

//    private string GenerateUrl(string text)
//    {
//        return $"https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen={text.Length}&client=tw-ob&q={WWW.EscapeURL(text)}&tl=ko";
//    }
//}

/*
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class GoogleTranslateTTS : MonoBehaviour {
    public static GoogleTranslateTTS Instance;
    public AudioSource audioSource;
    public int maxLength = 100; // Maximum length for TTS

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void ConvertAndPlay(string textToSpeak) {
        // Split the text into sentences or other logical units
        List<string> sentences = new List<string>();
        string[] words = textToSpeak.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string sentence = "";

        foreach (string word in words) {
            if ((sentence + word).Length > maxLength)
            {
                sentences.Add(sentence.Trim());
                sentence = "";
            }
            sentence += word + " ";
        }

        if (sentence.Length > 0)
            sentences.Add(sentence.Trim());

        StartCoroutine(DownloadAndPlayAudio(sentences));
    }

    IEnumerator DownloadAndPlayAudio(List<string> sentences) {
        List<AudioClip> clips = new List<AudioClip>();

        foreach (string sentence in sentences) {
            string url = GenerateUrl(sentence);
            AudioClip clip = null;
            Debug.Log("downloadandplay:" + sentence);

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG)) {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success) {
                    Debug.Log("downloaded clip");
                    clip = DownloadHandlerAudioClip.GetContent(www);
                }
                else {
                    Debug.LogError($"Failed to download audio clip: {www.error}");
                    yield break;
                }
            }

            if (clip != null)
            {
                clips.Add(clip);
            }
        }

        AudioClip combinedClip = CombineAudioClips(clips);
        if (combinedClip != null && audioSource != null) {
            audioSource.clip = combinedClip;
            Debug.Log("Play Audio");
            audioSource.Play();
        }
        else {
            Debug.Log("Failed to play audio. Combined clip is null or AudioSource is not assigned.");
        }
    }

    AudioClip CombineAudioClips(List<AudioClip> clips) {
        if (clips.Count == 0) return null;

        int combinedLength = 0;
        foreach (var clip in clips)
            combinedLength += clip.samples;

        AudioClip combinedClip = AudioClip.Create("Combined Clip", combinedLength, 1, 22050, false);
        float[] buffer = new float[combinedLength];
        int position = 0;

        foreach (var clip in clips) {
            float[] data = new float[clip.samples];
            clip.GetData(data, 0);
            Array.Copy(data, 0, buffer, position, data.Length);
            position += data.Length;
        }
        combinedClip.SetData(buffer, 0);
        return combinedClip;
    }

    private string GenerateUrl(string text) {
        return $"https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen={text.Length}&client=tw-ob&q={WWW.EscapeURL(text)}&tl=ko";
    }
}
*/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using OpenAI;

public class GoogleTranslateTTS : MonoBehaviour
{
    public static GoogleTranslateTTS Instance;
    public AudioSource audioSource;
    public int maxLength = 100;

    private AudioRoutineMng audiormng;
    public GameObject ObjectRoutineManager;
    private ChatManager chatMng;
    public GameObject ChatMngObj;
    private string clientId = "";
    //private string clientId = "";
    private string clientSecret = "";
    //private string clientSecret = "";

    string filePath = "D:/repos/temp";
    private void Start()
    {
        chatMng = ChatMngObj.GetComponent<ChatManager>();
        audiormng = ObjectRoutineManager.GetComponent<AudioRoutineMng>();
    }

    private int GetStatueIdxfromChatMng()
    {
        return chatMng.getCurrentStateIdx();
    }

    private void Update()
    {
        /*
        if (audioSource != null && !audioSource.isPlaying)
        {
            // Call your function here
            OnAudioFinished();
        }
        */
    }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //public void ConvertAndPlay(string textToSpeak) Ŭ�ι� ���� �� ���峪���� ����
    //{
    //    // Split the text into sentences or other logical units
    //    List<string> sentences = new List<string>();
    //    string[] words = textToSpeak.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //    string sentence = "";

    //    foreach (string word in words)
    //    {
    //        if ((sentence + word).Length > maxLength)
    //        {
    //            sentences.Add(sentence.Trim());
    //            sentence = "";
    //        }
    //        sentence += word + " ";
    //    }

    //    if (sentence.Length > 0)
    //        sentences.Add(sentence.Trim());

    //    StartCoroutine(DownloadAndPlayAudio(sentences));
    //}

    public void PlayIntro()
    {
        StartCoroutine(PlayAudioFromPath("D:/repos/temp/intro.mp3"));
    }
    

    public void PlayDialogues(List<string> dialogueList)
    {
        audiormng.AddtoDialogueQueue(dialogueList);
    }


    public void ConvertAndPlay(string textToSpeak) // ���� ������ ����
    {
        SomeSynchronousMethod(textToSpeak);
    }




    public void QueueListOfDialogue(string[] dialogueList)
    {

    }

    public async Task SendTTSGenerationRequestToClova()
    {
        // if dialogue queue is empty, end task; 

        // pops a dialogue request from list 
        // send request 


        // On reponse play the audio
    }

    public async Task PlayAudioAndEndTaskifAudioFinished()
    {
        // plays a audio if audio exist

        // awaits the end of the current list

        // if next dialgue exist create task SendTTSGenerationRequestToClova
        // else finish
    }

    public void SomeSynchronousMethod(string text)
    {
        string textToSpeech = text;

        // Run the asynchronous method on a separate thread and wait for it to complete
        Task.Run(async () =>
        {
            await DownloadAndPlayAudioAsync(textToSpeech);
            Console.WriteLine("The task has been finished."); // Message after the task completes
        }).Wait();

        // Other code here will execute after the async method and the message output completes


        // ����� ��� ����
        StartCoroutine(PlayAudioFromPath(filePath));
    }
    public async Task DownloadAndPlayAudioAsync(string sentence)
    {
        using (HttpClient client = new HttpClient())
        {
            int statueIdx = GetStatueIdxfromChatMng();
            Console.Write($"Current Statue index : {statueIdx}");
            var content = new StringContent($"speaker=nwontak&volume=0&speed=0&pistch=0&alpha=5&format=mp3&text={sentence}",
                                            Encoding.UTF8,
                                            "application/x-www-form-urlencoded");

            // Add headers
            client.DefaultRequestHeaders.Add("X-NCP-APIGW-API-KEY-ID", clientId);
            client.DefaultRequestHeaders.Add("X-NCP-APIGW-API-KEY", clientSecret);

            // Send a POST request
            HttpResponseMessage response = await client.PostAsync("https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts", content);

            if (response.IsSuccessStatusCode)
            {
                using (Stream input = await response.Content.ReadAsStreamAsync())
                using (FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await input.CopyToAsync(output);
                }
                Console.WriteLine(filePath + " was created");

            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }

    IEnumerator PlayAudioFromPath(string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    // Debug.Log("Audio played successfully.");
                }
            }
            else
            {
                // Debug.LogError("Error loading audio: " + www.error);
            }
        }
    }



    /*
    IEnumerator DownloadAndPlayAudio(List<string> sentences)
    {
        List<AudioClip> clips = new List<AudioClip>();

        foreach (string sentence in sentences)
        {
            string text = sentence; // �����ռ��� ���ڰ�
            string url = "https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-NCP-APIGW-API-KEY-ID", clientId);
            request.Headers.Add("X-NCP-APIGW-API-KEY", clientSecret);
            request.Method = "POST";
            byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=nwontak&volume=0&speed=0&pitch=0&format=mp3&text=" + text);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;
            Stream st = request.GetRequestStream();
            st.Write(byteDataParams, 0, byteDataParams.Length);
            st.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string status = response.StatusCode.ToString();
            Console.WriteLine("status=" + status);
            // using (Stream output = File.OpenWrite("c:/tts.mp3"))
            using (Stream output = File.OpenWrite("c:/repos/temp/temp.mp3"))
            
            using (Stream input = response.GetResponseStream())
            {
                input.CopyTo(output);
            }
            Console.WriteLine("c:/tts.mp3 was created");

            yield break;
        }
        AudioClip combinedClip = CombineAudioClips(clips);
        if (combinedClip != null && audioSource != null)
        {
            audioSource.clip = combinedClip;
            Debug.Log("Play Audio");
            audioSource.Play();
        }
        else
        {
            Debug.Log("Failed to play audio. Combined clip is null or AudioSource is not assigned.");
        }
    }
    */
    /*
    AudioClip CombineAudioClips(List<AudioClip> clips)
    {
        if (clips.Count == 0) return null;

        int combinedLength = 0;
        foreach (var clip in clips)
            combinedLength += clip.samples;

        AudioClip combinedClip = AudioClip.Create("Combined Clip", combinedLength, 1, 22050, false);
        float[] buffer = new float[combinedLength];
        int position = 0;

        foreach (var clip in clips)
        {
            float[] data = new float[clip.samples];
            clip.GetData(data, 0);
            Array.Copy(data, 0, buffer, position, data.Length);
            position += data.Length;
        }
        combinedClip.SetData(buffer, 0);
        return combinedClip;
    }
    */
    /*
    private string GenerateUrl(string text)
    {
        // Modify this method to return the appropriate URL for Clova AI Voice
        //return "http://tts.capstone.kr"; // Adjust as needed
        return "https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts";
    }
    */
}