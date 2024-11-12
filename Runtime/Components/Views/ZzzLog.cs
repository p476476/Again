using System.Collections;
using UnityEngine;

namespace Again.Runtime.Components.Views
{
    public class ZzzLog : MonoBehaviour
    {
        private const uint Qsize = 15; // number of messages to keep
        private readonly Queue _myLogQueue = new();

        private void Start()
        {
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        public void Clear()
        {
            _myLogQueue.Clear();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(50, 0, 1000, Screen.height));
            var guiStyle = new GUIStyle();
            guiStyle.fontSize = 30;
            guiStyle.normal.textColor = Color.white;
            GUILayout.Label("\n" + string.Join("\n", _myLogQueue.ToArray()), guiStyle);
            GUILayout.EndArea();
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Warning) return;
            _myLogQueue.Enqueue("[" + type + "] : " + logString);
            if (type == LogType.Exception)
                _myLogQueue.Enqueue(stackTrace);
            while (_myLogQueue.Count > Qsize)
                _myLogQueue.Dequeue();
        }
    }
}