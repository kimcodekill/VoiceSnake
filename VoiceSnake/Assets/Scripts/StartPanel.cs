using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class StartPanel : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject calibrationMenu;
    [Header("Main Menu Inputs")]
    [SerializeField] private InputField inputField;
    [SerializeField] private Dropdown sessionDropdown;
    [SerializeField] private Button startButton;
    [SerializeField] private Button calibrationMenuButton;
    [Header("Calibration Menu Inputs")]
    [SerializeField] private Dropdown commandsDropdown;
    [SerializeField] private Button startCalibrateButton;
    [SerializeField] private Button calibrationBackButton;
    [SerializeField] private Text textField;
    [SerializeField] private Toggle useCalibratedKeywords;
    [Header("Game")] 
    [SerializeField] private GameObject grid;
    [Header("Stuff for calibration ")]
    private DictationRecognizer dictationRecognizer;
    private Dictionary<string, List<string>> commandKeywords = new Dictionary<string, List<string>>();

    private string chosenCommand = "";

    public Dictionary<string, List<string>> CommandKeywords { get => commandKeywords; private set { } }

    public Toggle UseCalibratedKeywords { get => useCalibratedKeywords; private set { } }

    public void OnStart()
    {
        DataCollector.CreateDataSheet(inputField.text, (Session)(sessionDropdown.value - 1));
        grid.SetActive(true);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        startButton.interactable = (inputField.text.Length > 0 && (sessionDropdown.value - 1) != -1);
    }

    public void OnCalibrationMenuButton()
    {
        calibrationMenu.SetActive(true);
        mainMenu.SetActive(false);

        dictationRecognizer = new DictationRecognizer(ConfidenceLevel.Low);

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);

            // Lägg till i vald keyword lista
            //if (chosenCommand == "up")
            //{
            //   upKeywords.Add(text);
            //   StringBuilder stringBuilder = new StringBuilder(9999);
            //   foreach (string keyword in upKeywords)
            //       stringBuilder.Append(keyword + " ");
            //   textField.text = stringBuilder.ToString();
            //}
            commandKeywords[chosenCommand].Add(text);

            textField.text = "";
            int count = 0;
            foreach (string keyword in commandKeywords[chosenCommand])
            {
                count++;
                textField.text += keyword + " ";
            }
            textField.text += "\nTOTAL NO. OF KEYWORDS = " + count;

            dictationRecognizer.Stop();
            startCalibrateButton.interactable = true;
            startCalibrateButton.GetComponentInChildren<Text>().text = "Start Calibrate";
        };

        dictationRecognizer.DictationHypothesis += (text) =>
        {
            print($"Hypothesis: {text}");
        };

        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            print(completionCause);
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
            textField.text = $"Dictation error: {error}; HResult = {hresult}. ";
        };
    }

    public void OnCalibrationBackButton()
    {
        calibrationMenu.SetActive(false);
        mainMenu.SetActive(true);
        dictationRecognizer.Dispose();
    }

    public void OnStartCalibrateButton()
    {
        if (chosenCommand == "")
            return;

        startCalibrateButton.interactable = false;
        dictationRecognizer.Start();
        startCalibrateButton.GetComponentInChildren<Text>().text = "Listening..";
    }

    public void OnCommandDropdownChange()
    {
        if ((commandsDropdown.value - 1) == 0)
            chosenCommand = "up";
        else if ((commandsDropdown.value - 1) == 1)
            chosenCommand = "down";
        else if ((commandsDropdown.value - 1) == 2)
            chosenCommand = "left";
        else if ((commandsDropdown.value - 1) == 3)
            chosenCommand = "right";
        else
            chosenCommand = "";

        if (chosenCommand != "" && !commandKeywords.ContainsKey(chosenCommand))
            commandKeywords.Add(chosenCommand, new List<string>());

    }

    private void OnDestroy()
    {
        dictationRecognizer.Stop();
        dictationRecognizer.Dispose();
    }
}
