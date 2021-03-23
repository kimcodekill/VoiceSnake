using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceSnakeController : MonoBehaviour
{
    [SerializeField] private ConfidenceLevel minimumConfidenceLevel = ConfidenceLevel.Medium;
    [SerializeField] private bool useDictationRecognizer = false;
    private Snake snake;
    private DictationRecognizer dictationRecognizer;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private string[] keywords = { "up", "down", "left", "right"};

    private bool userSpeaking = false;
    private float startTime = 0;
    
    void Start()
    {
        snake = GetComponent<GridHandler>().GetSnake();
        
        actions.Add("up", Up);
        actions.Add("down", Down);
        actions.Add("left", Left);
        actions.Add("right", Right);

        if(useDictationRecognizer)
            CreateDictationRecognizer();
        else
            CreateKeywordRecognizer();
    }

    private void CreateKeywordRecognizer()
    {
        keywordRecognizer = new KeywordRecognizer(keywords, minimumConfidenceLevel);
        
        keywordRecognizer.OnPhraseRecognized += delegate(PhraseRecognizedEventArgs args)
        {
            // print($"Starttime: {args.phraseStartTime}");
            // print($"Endtime: {DateTime.Now}");
            TimeSpan delta = (DateTime.Now - args.phraseStartTime);
            TimeSpan netTime = delta - args.phraseDuration;
            // print($"Command: {args.text}");
            // print($"(delta: {delta.TotalMilliseconds}ms)");
            // print($"(netTime: {netTime.TotalMilliseconds}ms)");
            // print($"(phraseDuration: {args.phraseDuration.TotalMilliseconds}ms)");
            DataCollector.AddDataPoint(new DataPoint(
                EventType.Command, 
                args.text, 
                snake.Handler.CollectedApples,
                snake.Handler.StepDelay,
                netTime.TotalMilliseconds,
                args.phraseDuration.TotalMilliseconds,
                delta.TotalMilliseconds,
                -1f));

            actions[args.text].Invoke();
        };
        
        keywordRecognizer.Start();
    }

    private void CreateDictationRecognizer()
    {
        dictationRecognizer = new DictationRecognizer(minimumConfidenceLevel);
        
        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            if (actions.TryGetValue(text, out Action action))
            {
                action();
                print($"Moved {text}");
            }
            else
                print($"Keyword \"{text}\" not recognized");

            if (userSpeaking)
            {
                userSpeaking = false;
                print($"Delay: {(int)((Time.time - startTime) * 1000f)} ms");
            }
        };

        dictationRecognizer.DictationHypothesis += (text) =>
        {
            print($"Hypothesis: {text}");
            
            if (!userSpeaking)
            {
                userSpeaking = true;
                startTime = Time.time;
            }
        };

        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            print(completionCause);
            
            dictationRecognizer.Stop();
            dictationRecognizer.Start();
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };
        
        dictationRecognizer.Start();
    }

    private void Up() => snake.MoveDir = Vector2.down; // up and down are flipped because grid (0,0) is top left 
    private void Down() => snake.MoveDir = Vector2.up; 
    private void Left() => snake.MoveDir = Vector2.left;
    private void Right() => snake.MoveDir = Vector2.right;

    private void OnDestroy()
    {
        keywordRecognizer.Stop();
        keywordRecognizer.Dispose();
    }
}
