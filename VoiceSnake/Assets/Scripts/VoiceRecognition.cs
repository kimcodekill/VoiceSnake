using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech; //we cant use windows speech on android :shrug: couldve guessed

public class VoiceRecognition : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    
    void Start()
    {
        actions.Add("up", Up);
        actions.Add("down", Down);
        actions.Add("left", Left);
        actions.Add("right", Right);
        
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
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
}
