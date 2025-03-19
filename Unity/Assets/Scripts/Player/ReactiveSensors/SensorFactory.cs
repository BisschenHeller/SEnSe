using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SensorFactory : MonoBehaviour
{
    public List<SensorDictElement> sensorPrefabs;

    private List<ReactiveSensor> reactiveSensors;

    public ReactiveSensor CreateSensor(SensorID id)
    {
        switch (id)
        {
            case SensorID.AnimationFinished:
                GameObject container = new GameObject("WaitOnAnimationFinishedSensor");
                container.transform.parent = transform;
                container.AddComponent<WaitForAnimationSensor>();
                return container.GetComponent<WaitForAnimationSensor>();
            default:
                throw new Exception("Cannot create " + id.ToString() + "-Sensors dynamically (yet).");
        }
    }

    public void RemoveSensor(SensorID id)
    {
        ReactiveSensor found = reactiveSensors.Find(n => n.GetSensorID() == id);
        if (found != null) Destroy(found.gameObject);
    }

    private void Awake()
    {
        reactiveSensors = FindObjectsOfType<ReactiveSensor>().ToList();
    }
}

[Serializable]
public class SensorDictElement
{

}