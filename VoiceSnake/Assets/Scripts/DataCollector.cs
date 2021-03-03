using System;
using System.Collections.Generic;
using UnityEngine;

public class DataCollector
{
    private static Guid guid;
    private static List<DataPoint> dataPoints;

    public static void CreateDataSheet()
    {
        guid = Guid.NewGuid();
        Debug.Log(guid);
        dataPoints = new List<DataPoint>();
    }

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
            Debug.Log($"Event: {d.eventName}, Delay: {d.msDelay}");
        }
    }
}

//todo Expand this with more data
public struct DataPoint
{
    public string eventName { get; private set; }
    public float msDelay { get; private set; }

    public DataPoint(string eventName, float msDelay) => (this.eventName, this.msDelay) = (eventName, msDelay);
}
