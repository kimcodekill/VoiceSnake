using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class DataCollector
{
    private static string pseudonym;
    private static List<DataPoint> dataPoints;
    private static Session session;

    public static void CreateDataSheet(string user, Session s)
    {
        pseudonym = user;
        session = s;
        dataPoints = new List<DataPoint>();
    }

    public static void ClearDataPoints() => dataPoints = new List<DataPoint>();

    public static void AddDataPoint(DataPoint dataPoint)
    {
        if(dataPoints == null)
            Debug.LogError("No current datasheet");
        else
            dataPoints.Add(dataPoint);
    }

    public static void Save()
    {
        if(dataPoints == null)
            Debug.LogError("DATACOLLECTOR ERROR -> No current datasheet");
        //todo save as xml to file system
        //for now just print data points
        Debug.Log("Datapoints:");
        foreach (DataPoint d in dataPoints)
        {
            Debug.Log($"EventType: {d.eventType.ToString()} Event: {d.eventName}, Delay: {d.inputDelay}");
        }
    }
}

//todo Expand this with more data
public struct DataPoint
{
    public EventType eventType { get; private set; }
    public string eventName { get; private set; } 
    public int collectedFruits { get; private set; } 
    public float stepDelay { get; private set; } //ms
    public double inputDelay { get; private set; } //ms

    public DataPoint(EventType eventType, string eventName, int collectedFruits, float stepDelay, double inputDelay)
    {
        this.eventType = eventType;
        this.eventName = eventName;
        this.collectedFruits = collectedFruits;
        this.stepDelay = stepDelay;
        this.inputDelay = inputDelay;
    }
}

public enum EventType
{
    Command,
    PlayerState,
    whateverelse
}

public enum Session
{
    Session1Control, //A
    Session2Control, //B
    Session1Experiment, //C
    Session2Experiment //D 
}