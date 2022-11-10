using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugDisplay : MonoBehaviour
{
    Dictionary<string, string> debugLogs = new Dictionary<string, string>();
    public TextMeshProUGUI display;
    private void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }
    private void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type){
        display.text = $"{logString}\n{stackTrace}";
    }
}
