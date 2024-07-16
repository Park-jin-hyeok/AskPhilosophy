/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using System;
using System.Text;
using System.Collections.Specialized;
public class ResponseData
{
    public string PhilosopherName { get; set; }
    public string Utterance { get; set; }
    public int PhilosopherIndex { get; set; }
}

public class ChatManager : MonoBehaviour
{
    public OnResponseEvent OnResponse;
    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }
    public List<string> philosophers = new List<string> { "Plato", "Seneca", "Aristotle", "Descartes", "Confucius" };
    // reference to mouth objects
    public List<MouthAnimator> mouthControllers;

    private OpenAIApi openAI = new OpenAIApi();

    private List<ChatMessage> memory = new List<ChatMessage>();

    private ChatMessage mode;
    private ChatMessage end_prompt;

    private int responseNum = 0;

    public Queue<int> UtterancePhiloQueue = new Queue<int>();

    public List<ChatMessage> AddSystemMessage(ChatMessage newElement, ChatMessage endPrompt)
    {
        // Create a new list that is a copy of the existing memory
        List<ChatMessage> modifiedMemory = new List<ChatMessage>(memory);

        // Insert the new element at the start of the new list
        modifiedMemory.Insert(0, newElement);
        modifiedMemory.Add(endPrompt);

        return modifiedMemory;
    }
    private void AddNewMessagetoMemory(ChatMessage message)
    {
        // check memory length 
        if (memory.Count < 10)
        {
            memory.Add(message);
        }
        else
        {
            memory.RemoveAt(0);
            memory.Add(message);
        }
    }

    IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(5.0f);
    }

    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        List<ChatMessage> reqMessage;
        newMessage.Content = newText;
        newMessage.Role = "user";

        AddNewMessagetoMemory(newMessage);
        reqMessage = AddSystemMessage(mode, end_prompt);

        for (int i = 0; i < reqMessage.Count; i++)
        {
            Debug.Log("Message " + i + ": ");
            Debug.Log("Role: " + reqMessage[i].Role);
            Debug.Log("Content: " + reqMessage[i].Content);
        }


        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = reqMessage;
        request.Model = "gpt-4";
        request.MaxTokens = 150;

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            AddNewMessagetoMemory(chatResponse);

            List<ResponseData> parsedDataList = ParseResponse(chatResponse.Content);

            foreach (var parsedData in parsedDataList)
            {
                Debug.Log("Philosopher: " + parsedData.PhilosopherName);
                Debug.Log("Utterance: " + parsedData.Utterance);
                Debug.Log("Index: " + parsedData.PhilosopherIndex);

                UtterancePhiloQueue.Enqueue(parsedData.PhilosopherIndex);

                // Moved the method calls inside the loop
                PhilosopherDequeueAndToggleLipSync(parsedData.PhilosopherIndex); // Corrected the call with the index argument
                OnResponse.Invoke(parsedData.Utterance); // Invoke only the utterance
            }
        }
    }

    public List<ResponseData> ParseResponse(string responseContent)
    {
        var responses = new List<ResponseData>();
        string[] lines = responseContent.Split('\n');

        foreach (var line in lines)
        {
            int colonIndex = line.IndexOf(":");
            if (colonIndex != -1)
            {
                string philosopherName = line.Substring(0, colonIndex).Trim();
                string utterance = line.Substring(colonIndex + 1).Trim();

                int philosopherIndex = philosophers.IndexOf(philosopherName);

                responses.Add(new ResponseData
                {
                    PhilosopherName = philosopherName,
                    Utterance = utterance,
                    PhilosopherIndex = philosopherIndex
                });
            }
            else
            {
                responses.Add(new ResponseData
                {
                    PhilosopherName = "",
                    Utterance = line,
                    PhilosopherIndex = -1
                });
            }
        }

        return responses;
    }

    public void EnableAllLipSync()
    {
        foreach (var controller in mouthControllers)
        {
            controller.lipSyncToggle = true;
        }
    }

    public void DisableAllLipSync()
    {
        foreach (var controller in mouthControllers)
        {
            controller.lipSyncToggle = false;
        }

    }

    public void ToggleLipSyncForController(int index, bool state)
    {
        if (index >= 0 && index < mouthControllers.Count)
        {
            mouthControllers[index].lipSyncToggle = state;
        }
        else
        {
        }
        responseNum++;
    }

    public void PhilosopherDequeueAndToggleLipSync(int index)
    { // call this function when utterance ends 
        DisableAllLipSync();
        ToggleLipSyncForController(index, true);
    }

    void Start()
    {
        OnResponse.AddListener(TextToSpeech);

        string philosophersString = string.Join(", ", philosophers);

        ChatMessage newMode = new ChatMessage();
        newMode.Content = "You are simulating how philosophers would have answers to problems that people living in our century might have. "
            + "One should philosophical perspectives while engaging in with the question and each others opinion."
            + "If there is a utterance of a questioner focus on answering it. Try to keep the answer breif as possible while being useful a single sentence."
            + $"Consider the historical background. \n philosopher list:{philosophersString}.";
        newMode.Role = "system";

        ChatMessage endPrompt = new ChatMessage();
        endPrompt.Content = "What would the philosophers say continue the dialogue? (guess who would talk by context)"
                            + "If the dialog doesn't end with the questioner, the next philosopher should start with having a opinion about the previous philosopher. "
                            + "please make the conversation natural and friendly, make them talk like they know they have been reincarnated to have the fasinating"
                            + $" chance to talk to each other. Choose only one of the philosopher from the list and speak as he would. list of philosophers: {philosophersString} \n"
                            + " answer in this kind of format. (example\nname: utterance)  ?          ";
        endPrompt.Content = $"Imagine a conversation between two philosophers from the provided list, where they engage in a friendly and insightful dialogue, there should be 2~4 utterances in sum. The philosophers, aware of their unique chance to converse after being reincarnated, should discuss a philosophical topic relevant to their historical perspectives. The dialogue should flow naturally, with each philosopher responding to or elaborating on the previous utterance. The format for this conversation should be as follows: each statement is prefaced by the philosopher's name, in a back-and-forth style. Example format: 'Philosopher Name: Utterance'. Ensure the conversation reflects a deep understanding of each philosopher's viewpoints and creates a dynamic exchange of ideas. Given list of philosopher: {philosophersString}";
        endPrompt.Role = "system";

        mode = newMode;
        end_prompt = endPrompt;
    }

    // Update is called once per frame
    void Update()
    {
        if (JointController.b == 1)
        {
            EnableAllLipSync();
            TextToSpeech("철학에 어떤 질문이 있으십니까?");
            JointController.b = 0;
            Debug.Log(JointController.b);
        }
    }

    void TextToSpeech(string responseText)
    {
        GoogleTranslateTTS.Instance.ConvertAndPlay(responseText);
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using System;
using System.Text;
using System.Collections.Specialized;

 

public class ResponseData
{
    public string PhilosopherName { get; set; }
    public string Utterance { get; set; }
    public int PhilosopherIndex { get; set; }
}

public class ChatManager : MonoBehaviour
{
    public OnResponseEvent OnResponse;
    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }
    public List<string> philosophers = new List<string> { "Plato", "Seneca", "Aristotle", "Descartes", "Confucius" };
    // reference to mouth objects
    public List<MouthAnimator> mouthControllers;
    public int nMemory = 2;

    private OpenAIApi openAI = new OpenAIApi();
    
    private List<ChatMessage> memory = new List<ChatMessage>();

    private ChatMessage mode;
    private ChatMessage end_prompt;

    private int responseNum = 0;
    public int philnumber = -1;
    public int currentUttererIdx = 0;

    public Queue<int> UtterancePhiloQueue = new Queue<int>();

    public GameObject Aristo_head;
    public GameObject Seneka_head;

    public List<ChatMessage> AddSystemMessage(ChatMessage newElement, ChatMessage endPrompt)
    {
        // Create a new list that is a copy of the existing memory
        List<ChatMessage> modifiedMemory = new List<ChatMessage>(memory);

        // Insert the new element at the start of the new list
        modifiedMemory.Insert(0, newElement);
        modifiedMemory.Add(endPrompt);

        return modifiedMemory;
    }
    private void AddNewMessagetoMemory(ChatMessage message)
    {
        // check memory length 
        if (memory.Count < nMemory) {
            memory.Add(message);
        }
        else {
            memory.RemoveAt(0);
            memory.Add(message);
        }
    }

    //public async void AskChatGPT(string newText)
    //{
    //    ChatMessage newMessage = new ChatMessage();
    //    List<ChatMessage> reqMessage;
    //    newMessage.Content = newText;
    //    newMessage.Role = "user";

    //    Debug.Log(newText);
    //    messages.Add(newMessage); 

    //    CreateChatCompletionRequest request = new CreateChatCompletionRequest(); 
    //    request.Messages = messages;
    //    request.Model = "gpt-3.5-turbo";

    //    var response = await openAI.CreateChatCompletion(request);  

    //    if (response.Choices != null && response.Choices.Count > 0)
    //    {
    //        var chatResponse = response.Choices[0].Message;
    //        messages.Add(chatResponse); 

    //        Debug.Log(chatResponse.Content); 

    //        OnResponse.Invoke(chatResponse.Content);
    //    }
    //}

    IEnumerator WaitSeconds() {
        yield return new WaitForSeconds(5.0f);
    }

    public async void AskChatGPT(string newText)
    {
        string philname = null;

        if (philnumber == 0) {
            philname = "Aristotle";
        }
        else if (philnumber == 1) {
            philname = "Seneka";
        }

        ChatMessage newMessage = new ChatMessage();
        List<ChatMessage> reqMessage;

        if (philnumber != -1) {
            newMessage.Content = newText + $"({philname} should answer)";
        }
        else {
            newMessage.Content = newText;
        }
        
        newMessage.Role = "user";

        AddNewMessagetoMemory(newMessage);
        reqMessage = AddSystemMessage(mode, end_prompt);

        for (int i = 0; i < reqMessage.Count; i++)
        {
            Debug.Log("Message " + i + ": ");
            Debug.Log("Role: " + reqMessage[i].Role);
            Debug.Log("Content: " + reqMessage[i].Content);
            // Add any other properties of ChatMessage you wish to print
        }


        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = reqMessage;
        request.Model = "gpt-4";
        // request.Model = "gpt-3.5-turbo";
        request.MaxTokens = 500;

        Aristo_head.GetComponent<NoddingAnim>().AristoThinking(true);
        Seneka_head.GetComponent<NoddingAnim>().SenekaThinking(true);

        var response = await openAI.CreateChatCompletion(request);

        Aristo_head.GetComponent<NoddingAnim>().AristoThinking(false);
        Seneka_head.GetComponent<NoddingAnim>().SenekaThinking(false);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            AddNewMessagetoMemory(chatResponse);

            List<ResponseData> parsedDataList = ParseResponse(chatResponse.Content);
            List<string> utteranceList = new List<string>();

            foreach (var parsedData in parsedDataList)
            {
                Debug.Log("Philosopher: " + parsedData.PhilosopherName);
                Debug.Log("Utterance: " + parsedData.Utterance);
                Debug.Log("Index: " + parsedData.PhilosopherIndex);

                utteranceList.Add(parsedData.Utterance);
                UtterancePhiloQueue.Enqueue(parsedData.PhilosopherIndex);
            }
            // this should not(!) be move inside the loop, iteration is initiated by the async tasks
            PhilosopherDequeueAndToggleLipSync(); // the index is not declaired
            TextToSpeech(utteranceList);
        }
    }

    public List<ResponseData> ParseResponse(string responseContent)
    {
        var responses = new List<ResponseData>();
        string[] lines = responseContent.Split('\n');

        string currentPhilosopherName = string.Empty;
        StringBuilder currentUtterance = new StringBuilder();
        int currentPhilosopherIndex = -1;

        foreach (var line in lines)
        {
            Debug.Log(line);
            int colonIndex = line.IndexOf(":");
            if (colonIndex != -1)
            {
                // If there is a current utterance being built, add it before starting a new one
                if (currentUtterance.Length > 0)
                {
                    responses.Add(new ResponseData
                    {
                        PhilosopherName = currentPhilosopherName,
                        Utterance = currentUtterance.ToString(),
                        PhilosopherIndex = currentPhilosopherIndex
                    }
                    );
                    currentUtterance.Clear();
                }

                currentPhilosopherName = line.Substring(0, colonIndex).Trim();
                currentPhilosopherIndex = philosophers.IndexOf(currentPhilosopherName);
                currentUtterance.Append(line.Substring(colonIndex + 1).Trim());
            }
            else
            {
                // If the line doesn't contain a colon, append it to the current utterance
                if (currentUtterance.Length > 0)
                    currentUtterance.Append(" "); // Add a space for separation

                currentUtterance.Append(line.Trim());
            }
        }

        // Add the last utterance if it exists
        if (currentUtterance.Length > 0)
        {
            responses.Add(new ResponseData
            {
                PhilosopherName = currentPhilosopherName,
                Utterance = currentUtterance.ToString(),
                PhilosopherIndex = currentPhilosopherIndex
            });
        }

        return responses;
    }


    // public ResponseData ParseResponse(string responseContent)
    // {
    //     int colonIndex = responseContent.IndexOf(":");
    //     if (colonIndex != -1)
    //     {
    //         string philosopherName = responseContent.Substring(0, colonIndex).Trim();
    //         string utterance = responseContent.Substring(colonIndex + 1).Trim();

    //         int philosopherIndex = philosophers.IndexOf(philosopherName);

    //         return new ResponseData
    //         {
    //             PhilosopherName = philosopherName,
    //             Utterance = utterance,
    //             PhilosopherIndex = philosopherIndex
    //         };
    //     }
    //     else
    //     {
    //         return new ResponseData
    //         {
    //             PhilosopherName = "",
    //             Utterance = responseContent,
    //             PhilosopherIndex = -1
    //         };
    //     }
    // }

    // Method to enable lip sync for all controllers
    public void EnableAllLipSync()
    {
        foreach (var controller in mouthControllers)
        {
            controller.lipSyncToggle = true;
            // Any additional code to enable lip sync
        }
    }

    // Method to disable lip sync for all controllers
    public void DisableAllLipSync()
    {
        foreach (var controller in mouthControllers)
        {
            controller.lipSyncToggle = false;
            // Any additional code to disable lip sync
        }
        
    }

    // Method to toggle lip sync for a specific controller
    public void ToggleLipSyncForController(int index, bool state)
    {
        if (index >= 0 && index < mouthControllers.Count)
        {
            mouthControllers[index].lipSyncToggle = state;
            // Any additional code to toggle lip sync
        }
        else
        {
            // Debug.LogError("Index out of range: " + index);
        }
        responseNum++;
    }

    public int getCurrentStateIdx()
    {
        return currentUttererIdx;
    }

    public void PhilosopherDequeueAndToggleLipSync(){ // call this function when utterance  
        try
        {
            int index = UtterancePhiloQueue.Dequeue();
            currentUttererIdx = index;
            DisableAllLipSync();
            ToggleLipSyncForController(index, true);
        }
        catch
        {
            return;
        }
        
    }

    void Start()
    {

        // philosopher setting
        string philosophersString = string.Join(", ", philosophers);

        ChatMessage newMode = new ChatMessage();
        newMode.Content = "You are simulating how philosophers would have answers to problems that people living in our century might have. "
            + "One should philosophical perspectives while engaging in with the question and each others opinion."
            + "If there is a utterance of a questioner focus on answering it. Try to keep the answer breif as possible while being useful a single sentence."
            + $"Consider the historical background. \n philosopher list:{philosophersString}.";
        newMode.Role = "system";

        ChatMessage endPrompt = new ChatMessage();
        //endPrompt.Content = "What would one of the philosophers say continue the dialogue? (guess who would talk by context)"
        //                    + "If the dialog doesn't end with the questioner, the next philosopher should start with having a opinion about the previous philosopher. "
        //                    + "please make the conversation natural and friendly, make them talk like they know they have been reincarnated to have the fasinating"
        //                    + $" chance to talk to each other. Choose only one of the philosopher from the list and speak as he would. list of philosophers: {philosophersString} \n"
        //                    + " answer in this kind of format. (example\nname: utterance)  ?          ";
        endPrompt.Content = "What would the philosophers say continue the dialogue? (guess who would talk by context)"
                            + "If the dialog doesn't end with the questioner, the next philosopher should start with having a opinion about the previous philosopher. "
                            + "please make the conversation natural and friendly, make them talk like they know they have been reincarnated to have the fasinating"
                            + $" chance to talk to each other. Choose only one of the philosopher from the list and speak as he would. list of philosophers: {philosophersString} \n"
                            + " answer in this kind of format. (example\nname: utterance)  ?          ";
        endPrompt.Content = $"Imagine a conversation between two philosophers from the provided list, where they engage in a friendly and insightful dialogue, there should be 4~5 utterances in sum. The philosophers, aware of their unique chance to converse after being reincarnated, should discuss a philosophical topic relevant to their historical perspectives. The dialogue should flow naturally, with each philosopher responding to or elaborating on the previous utterance. The format for this conversation should be as follows: each statement is prefaced by the philosopher's name, in a back-and-forth style. Example format: 'Philosopher Name: Utterance'. Ensure the conversation reflects a deep understanding of each philosopher's viewpoints and creates a dynamic exchange of ideas. Given list of philosopher: {philosophersString}  한국어로 대답을 해줘";
        endPrompt.Role = "system";

        mode = newMode;
        end_prompt = endPrompt;

        //      ?    ??     ?   ?  ;  ? 
        //GoogleTranslateTTS.Instance.ConvertAndPlay("? п   ?    ?       ?? ?");
        //EnableAllLipSync();
        //TextToSpeech("? п   ?    ?       ?? ?");
    }

    // Update is called once per frame
    void Update()
    {
        if (JointController.b == 1) {
            Aristo_head.GetComponent<NoddingAnim>().AristoSleeping(false);
            Seneka_head.GetComponent<NoddingAnim>().SenekaSleeping(false);
            /*
            if (Aristo_head.GetComponent<NoddingAnim>().animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                Seneka_head.GetComponent<NoddingAnim>().animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
                EnableAllLipSync();
                TextToSpeech(new List<string> { "철학에 대해 어떤 질문이 있으신가요?" });
                JointController.b = 0;
                Debug.Log(JointController.b);
            }
            */
            EnableAllLipSync();
            //TextToSpeech(new List<string> { "철학에 대해 어떤 질문이 있으신가요?" });
            GoogleTranslateTTS.Instance.PlayIntro();
            JointController.b = 0;
            Debug.Log(JointController.b);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Aristo_head.GetComponent<NoddingAnim>().AristoSleeping(false);
            Seneka_head.GetComponent<NoddingAnim>().SenekaSleeping(false);
            EnableAllLipSync();
            //TextToSpeech(new List<string> { "철학에 대해 어떤 질문이 있으신가요?" });
            GoogleTranslateTTS.Instance.PlayIntro();
            JointController.b = 0;
            Debug.Log(JointController.b);
        }
    }

    void TextToSpeech(List<string> responseText)
    {
        GoogleTranslateTTS.Instance.PlayDialogues(responseText);

        //GoogleTranslateTTS.Instance.ConvertAndPlay(responseText);
    }
}