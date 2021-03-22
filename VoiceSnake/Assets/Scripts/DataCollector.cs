using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DataCollector
{
    private static DataSheet dataSheet;

    public static void CreateDataSheet(string user, Session s)
    {
        dataSheet = new DataSheet(user, s);
    }

    public static void ClearDataPoints()
    {
        dataSheet.dataPoints = new List<DataPoint>();
    }

    public static void AddDataPoint(DataPoint dataPoint)
    {
        if(dataSheet.dataPoints == null)
            Debug.LogError("DATACOLLECTOR ERROR -> No current datasheet");
        else
            dataSheet.dataPoints.Add(dataPoint);
    }

    public static void Save()
    {
        if (dataSheet.dataPoints == null)
        {
            Debug.LogError("DATACOLLECTOR ERROR -> No current datasheet");
            return;
        }
        if(dataSheet.dataPoints.Count == 0)
            return;

#if UNITY_EDITOR
        string root = Application.dataPath + @"\TestData\";
#else  
        string root = AppContext.BaseDirectory + @"\TestData\";
#endif

        if (!Directory.Exists(root)) Directory.CreateDirectory(root);
        
        string fileName = root + $"{dataSheet.session.ToString()}_{dataSheet.pseudonym}.xml";
        Debug.Log(fileName);
        
        using (Stream stream = new FileStream(fileName, FileMode.Create))
        {
            XmlSerializer xml = new XmlSerializer(typeof(DataSheet));
            
            xml.Serialize(stream, dataSheet);
            // foreach (DataPoint d in dataPoints)
            // {
            //     xml.Serialize(stream, d);
            //     Debug.Log($"EventType: {d.eventType.ToString()} Event: {d.eventName}, Delay: {d.inputDelay}");
            // }
        }
    }
}

//todo Double check this is all the data we need
[Serializable]
public struct DataPoint
{
    public EventType eventType;
    public string eventName;
    public int collectedFruits;
    public float stepDelay; //ms
    public double inputDelay; //ms

    public DataPoint(EventType eventType, string eventName, int collectedFruits, float stepDelay, double inputDelay)
    {
        this.eventType = eventType;
        this.eventName = eventName;
        this.collectedFruits = collectedFruits;
        this.stepDelay = stepDelay;
        this.inputDelay = inputDelay;
    }
}

[Serializable]
public struct DataSheet
{
    public string pseudonym;
    public Session session;
    public List<DataPoint> dataPoints;

    public DataSheet(string pseudonym, Session session)
    {
        this.pseudonym = pseudonym;
        this.session = session;
        dataPoints = new List<DataPoint>();
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