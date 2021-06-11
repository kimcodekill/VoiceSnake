using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceSnakeController : MonoBehaviour
{
    [SerializeField] private ConfidenceLevel minimumConfidenceLevel = ConfidenceLevel.Medium;
    [SerializeField] private bool useDictationRecognizer = false;
    [SerializeField] private StartPanel startPanel;
    private Snake snake;
    private DictationRecognizer dictationRecognizer;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions;
    private Dictionary<List<string>, Action> calibratedActions;
    private AzureVoiceRecognition voiceRecognition;

    private string[] keywords = { "up", "down", "left", "right"};

    private bool userSpeaking = false;
    private float startTime = 0;

    private void Awake()
    {
        voiceRecognition = GetComponent<AzureVoiceRecognition>();
    }

    private void Update()
    {
        if (voiceRecognition.recognizer != null)
            CheckVoiceInput();
    }

    private void CheckVoiceInput()
    {
        //if (voiceRecognition.recognizedText.Length > 0 && voiceRecognition.recognitionDelta != TimeSpan.Zero)
        if(voiceRecognition.recognizedTexts.Count > 0)
        {
            (string text, double delta) q = voiceRecognition.recognizedTexts.Dequeue();
            
            //print($"text: {q.text}, delta: {q.delta}");
            
            //print($"text: {voiceRecognition.recognizedText} delta: {voiceRecognition.recognitionDelta.TotalMilliseconds:F2}");
            if (startPanel.UseCalibratedKeywords.isOn)
            {
                bool found = false;
                foreach (var listOfKeywords in calibratedActions.Keys)
                {
                    foreach (string keyword in listOfKeywords)
                    {
                        if (keyword == q.text && !found)
                        {
                            DataCollector.AddDataPoint(new DataPoint(
                                EventType.Command, 
                                q.text, 
                                snake.Handler.CollectedApples,
                                snake.Handler.StepDelay,
                                // -1f,
                                // -1f,
                                q.delta,
                                -1f));
                            
                            calibratedActions[listOfKeywords].Invoke();
                            //print($"Calibrated action");
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
            } 
            else if (actions.TryGetValue(q.text, out Action action))
            {
                DataCollector.AddDataPoint(new DataPoint(
                    EventType.Command, 
                    q.text, 
                    snake.Handler.CollectedApples,
                    snake.Handler.StepDelay,
                    // -1f,
                    // -1f,
                    q.delta,
                    -1f));
                
                action.Invoke();
            }
            
            voiceRecognition.recognitionDelta = TimeSpan.Zero;
            voiceRecognition.recognizedText = "";
        }
    }

    public void StartVoiceControl(Snake snake)
    {
        this.snake = snake;

        if (startPanel.UseCalibratedKeywords.isOn)
        {
            calibratedActions = new Dictionary<List<string>, Action>();
            calibratedActions.Add(startPanel.CommandKeywords["up"], Up);
            calibratedActions.Add(startPanel.CommandKeywords["down"], Down);
            calibratedActions.Add(startPanel.CommandKeywords["left"], Left);
            calibratedActions.Add(startPanel.CommandKeywords["right"], Right);
        }
        else
        {
            actions = new Dictionary<string, Action>();
            actions.Add("up", Up);
            actions.Add("down", Down);
            actions.Add("left", Left);
            actions.Add("right", Right);
        }

        if (voiceRecognition.recognizer == null && dictationRecognizer == null)
        {
            if(useDictationRecognizer)
                CreateDictationRecognizer();
            else
            {
                CreateSpeechRecognizer();
                //CreateKeywordRecognizer();
            }
        }
        else
        {
            if (useDictationRecognizer)
                dictationRecognizer.Start();
            else
                voiceRecognition.StartSpeechRecognizer();
            //keywordRecognizer.Start();
        }
    }

    private void CreateSpeechRecognizer()
    {
        voiceRecognition.CreateSpeechRecognizer();
        voiceRecognition.StartSpeechRecognizer();
    }

    public void StopVoiceControl()
    {
        if(useDictationRecognizer)
            dictationRecognizer.Stop();
        else
            voiceRecognition.StopRecognition();
            //keywordRecognizer.Stop();
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
                // netTime.TotalMilliseconds,
                // args.phraseDuration.TotalMilliseconds,
                delta.TotalMilliseconds,
                -1f));

            if (startPanel.UseCalibratedKeywords.isOn)
            {
                bool found = false;
                foreach (var listOfKeywords in calibratedActions.Keys)
                {
                    foreach (string keyword in listOfKeywords)
                    {
                        if (keyword == args.text && !found)
                        {
                            calibratedActions[listOfKeywords].Invoke();
                            print($"Calibrated action");
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
            } else
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

    private void Up() => snake.EnqueueMove(Vector2.down); // up and down are flipped because grid (0,0) is top left 
    private void Down() => snake.EnqueueMove(Vector2.up); 
    private void Left() => snake.EnqueueMove(Vector2.left);
    private void Right() => snake.EnqueueMove(Vector2.right);

    private void OnDestroy()
    {
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
}
