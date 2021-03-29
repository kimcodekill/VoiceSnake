using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CognitiveServices.Speech;
using UnityEngine;

public class AzureVoiceRecognition : MonoBehaviour
{
    [SerializeField] private string speechServiceAPIKey;
    [SerializeField] private string speechServiceRegion;

    private const string lang = "en-us";
    
    public SpeechRecognizer recognizer;
    public Dictionary<string, Action> actions;
    
    //for thread sanitation purposes
    [HideInInspector] public string recognizedText = "";
    [HideInInspector] public Queue<(string text, double delta)> recognizedTexts;
    public TimeSpan recognitionDelta = TimeSpan.Zero;
    private System.Object threadLocker = new System.Object();
    private TimeSpan prevDelay;

    public void CreateSpeechRecognizer()
    {
        if (recognizer == null)
        {
            print("creating recognizer");
            SpeechConfig config = SpeechConfig.FromSubscription(speechServiceAPIKey, speechServiceRegion);
            config.SpeechRecognitionLanguage = lang;
            recognizer = new SpeechRecognizer(config);
            recognizedTexts = new Queue<(string text, double delta)>();

            if (recognizer != null)
            {
                recognizer.Recognized += RecognizerOnRecognized;
                recognizer.Recognizing += RecognizerOnRecognizing;
                //recognizer.SpeechEndDetected +=RecognizerOnSpeechEndDetected;
            }
        }
    }

    private void RecognizerOnRecognizing(object sender, SpeechRecognitionEventArgs e)
    {
        if(e.Result.Reason != ResultReason.RecognizingSpeech)
            return;

        lock (threadLocker)
        {
            print($"Hypothesis: {e.Result.Text}");
            string text = Regex.Replace(e.Result.Text, @"[^0-9a-zA-Z ]+", "").ToLower();
            string[] snippets = text.Split(' ');

            TimeSpan delay;
            if (snippets.Length > 1)
                delay = e.Result.Duration - prevDelay;
                //print($"dur: {e.Result.Duration.TotalMilliseconds:F2}, actDur: {delay.TotalMilliseconds:F2}, prevDur: {prevDelay.TotalMilliseconds:F2}");
            else
                delay = e.Result.Duration;

            prevDelay = e.Result.Duration;

            int num = snippets.Length > 1 ? snippets.Length : 1;
            
            recognizedTexts.Enqueue((snippets.LastOrDefault(), delay.TotalMilliseconds));
        }
    }

    public async void StartSpeechRecognizer()
    {
        if (recognizer != null)
        {
            print("starting recognizer");
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
        }
    }

    private void RecognizerOnRecognized(object sender, SpeechRecognitionEventArgs e)
    {
        if(e.Result.Reason != ResultReason.RecognizedSpeech)
            return;
        
        lock (threadLocker)
        {
            // recognizedText = Regex.Replace(e.Result.Text, @"[^0-9a-zA-Z]+", "").ToLower();
            // recognitionDelta = e.Result.Duration;
        }
    }

    private void OnDisable()
    {
        StopRecognition();
        Dispose();
    }

    private void Dispose()
    {
        if (recognizer != null)
        {
            recognizer.Recognized -= RecognizerOnRecognized;
            recognizer.Recognizing -= RecognizerOnRecognizing;
            //recognizer.SpeechEndDetected -= RecognizerOnSpeechEndDetected;
            recognizer.Dispose();
            recognizer = null;
        }
    }

    public async void StopRecognition()
    {
        if (recognizer != null)
        {
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        }
    }
}
