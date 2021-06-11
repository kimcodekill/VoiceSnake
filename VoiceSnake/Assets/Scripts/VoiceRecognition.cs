using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech; //we cant use windows speech on android :shrug: couldve guessed
using static AndroidBridgeUtils;

public class VoiceRecognition : MonoBehaviour, IAndroidBridge
{
    private KeywordRecognizer keywordRecognizer;
    private DictationRecognizer dictationRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    
    void Start()
    {
    #if UNITY_EDITOR
        actions.Add("up", Up);
        actions.Add("down", Down);
        actions.Add("left", Left);
        actions.Add("right", Right);
        
        // cannot run multiple recognizers at once
        // keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        // keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        // keywordRecognizer.Start();
        
        dictationRecognizer = new DictationRecognizer();
        
        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            print(actions.TryGetValue(text, out Action action));
        };

        dictationRecognizer.DictationHypothesis += (text) =>
        {
            if(actions.TryGetValue(text, out Action action))
                action();  
        };

        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            // if (completionCause == DictationCompletionCause.TimeoutExceeded)
            // {
            //     print("restarting dictation");
            // }
            // else if (completionCause != DictationCompletionCause.Complete)
            //     Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            //
            dictationRecognizer.Stop();
            dictationRecognizer.Start();
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };
        dictationRecognizer.Start();
    #else
        SetContinuousListening(true);
        StartListening();
    #endif
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"{speech.text} ({speech.confidence})");
        actions[speech.text].Invoke();  
    } 

    private void Up() => transform.position += Vector3.up;
    private void Down() => transform.position += Vector3.down;
    private void Left() => transform.position += Vector3.left;
    private void Right() => transform.position += Vector3.right;
    
    private void StartListening()
    {
        AndroidRunnableCall("StartListening");
    }

    private void SetContinuousListening(bool isContinuous)
    {
        AndroidCall("SetContinuousListening", isContinuous);
    }

    public void OnResult(string recognizedResult)
    {
        char[] delimiterChars = { '~' };
        string[] result = recognizedResult.Split(delimiterChars);

        for (int i = 0; i < result.Length; i++)
        {
            print(result[i]);
            actions[result[i]].Invoke();
        }
    }
}
