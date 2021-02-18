using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows.Speech;

public class VoiceSnakeController : MonoBehaviour
{
    private Snake snake;
    private DictationRecognizer dictationRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    
    void Start()
    {
        snake = GetComponent<GridHandler>().GetSnake();
        
        actions.Add("up", Up);
        actions.Add("down", Down);
        actions.Add("left", Left);
        actions.Add("right", Right);
        
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
    }

    private void Up() => snake.MoveDir = Vector2.down;
    private void Down() => snake.MoveDir = Vector3.up; //these are flipped for some reason..
    private void Left() => snake.MoveDir = Vector3.left;
    private void Right() => snake.MoveDir = Vector3.right;
}
