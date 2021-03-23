using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputField inputField;
    [SerializeField] private Dropdown sessionDropdown;
    [SerializeField] private Button startButton;
    [Header("Game")] 
    [SerializeField] private GameObject grid; 

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
}
