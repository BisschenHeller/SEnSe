using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SensorDebug : MonoBehaviour
{
    private List<string> strings = new List<string>();
    private List<string> titles = new List<string>();
    private TextMeshProUGUI textField;

    private bool started = false;
    public bool refresh = false;

    public List<SensorID> sensorsToObserve = new List<SensorID>();
    public List<ReactiveSensor> sensorsFound = new List<ReactiveSensor>();

    public List<IDisposable> disposables = new List<IDisposable>();

    List<Action<Vector3>> vector3actions = new List<Action<Vector3>>();
    List<Action<bool>> boolActions = new List<Action<bool>>();
    List<Action<float>> floatActions = new List<Action<float>>();

    private void OnValidate()
    {
        if (!started || !Application.isPlaying) return;
        titles = new List<string>();
        strings = new List<string>();
        disposables.ForEach(n => n.Dispose());
        disposables = new List<IDisposable>();

        sensorsFound = FindObjectsOfType<ReactiveSensor>().Where(n => sensorsToObserve.Contains(n.GetSensorID())).ToList();

        if (sensorsFound.Count > 4) Debug.LogWarning("Can only debug-observe 4 Sensors at once.");

        for (int i = 0; i < Mathf.Min(4, sensorsFound.Count); i++)
        {
            titles.Add(sensorsFound[i].ToInspectorString());
            strings.Add("No Values Yet");
            strings.Add("No Values Yet");
            strings.Add("No Values Yet");
            try { disposables.Add(sensorsFound[i].SubscribeV(vector3actions[i])); } catch (Exception e) {strings[i*3] = "No Vec3 Overload (" + e.Message + ")"; }
            try { disposables.Add(sensorsFound[i].SubscribeB(boolActions[i])); } catch (Exception e) { strings[i * 3 + 1] = "No Bool Overload (" + e.Message + ")"; }
            try { disposables.Add(sensorsFound[i].SubscribeF(floatActions[i])); } catch (Exception e) { strings[i * 3 + 2] = "No Float Overload (" + e.Message + ")"; }
        }
    }

    void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        
        vector3actions = new List<Action<Vector3>>() { 
            SetVec3String0, 
            SetVec3String1, 
            SetVec3String2,
            SetVec3String3};
        boolActions = new List<Action<bool>>()
        {
            SetBoolString0,
            SetBoolString1,
            SetBoolString2,
            SetBoolString3
        };
        floatActions = new List<Action<float>>()
        {
            SetFloatString0,
            SetFloatString1,
            SetFloatString2,
            SetFloatString3
        };
        started = true;
    }

    void SetVec3String0(Vector3 vec) { strings[0] = vec.ToString(); }
    void SetBoolString0(bool b) { strings[1] = b.ToString(); }
    void SetFloatString0(float f) { strings[2] = f.ToString(); }
    void SetVec3String1(Vector3 vec) { strings[3] = vec.ToString(); }
    void SetBoolString1(bool b) { strings[4] = b.ToString(); }
    void SetFloatString1(float f) { strings[5] = f.ToString(); }
    void SetVec3String2(Vector3 vec) { strings[6] = vec.ToString(); }
    void SetBoolString2(bool b) { strings[7] = b.ToString(); }
    void SetFloatString2(float f) { strings[8] = f.ToString(); }
    void SetVec3String3(Vector3 vec) { strings[9] = vec.ToString(); }
    void SetBoolString3(bool b) { strings[10] = b.ToString(); }
    void SetFloatString3(float f) { strings[11] = f.ToString(); }
    

    // Update is called once per frame
    void Update()
    {
        if (refresh)
        {
            refresh = false;
            OnValidate();
        }
        string retText = "Sensors:\n";

        for (int i = 0; i < titles.Count; i++)
        {
            retText += "\n" + titles[i].ToString() + "\n";
            retText += "  <color=#aaaaaa>V3:</color><indent=20%>" + strings[3*i] + "</indent>\n";
            retText += "  <color=#aaaaaa>B:</color><indent=20%>" + strings[3 * i+1] + "</indent>\n";
            retText += "  <color=#aaaaaa>F:</color><indent=20%>" + strings[3 * i+2] + "</indent>\n";
        }

        textField.text = retText;
    }
}
