using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour
{
    [SerializeField] private Text appleLabel;
    [SerializeField] private GridHandler gridHandler;

    private void OnEnable()
    {
        appleLabel.text = gridHandler.CollectedApples.ToString();
    }

    public void Restart()
    {
        DataCollector.Save();
        SceneManager.LoadScene(0);
    }
}
