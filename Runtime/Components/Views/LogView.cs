using System.Collections.Generic;
using Again.Scripts.Runtime.Common;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    public class LogView : MonoBehaviour, ILogView
    {
        public Transform logContainer;
        public GameObject logPrefab;
        public List<LogBlock> logs;

        private void Awake()
        {
            transform.ResetAndHide();
        }

        public void Reset()
        {
            foreach (var log in logs)
                Destroy(log.gameObject);
            logs.Clear();
        }

        public void Add(DialogueLog log)
        {
            var logObject = Instantiate(logPrefab, logContainer);
            logObject.SetActive(true);
            var logBlock = logObject.GetComponent<LogBlock>();
            logBlock.SetName(log.CharacterKey);
            logBlock.SetContent(log.TextKey);
            logs.Add(logBlock);
        }

        public void SetLogs(List<DialogueLog> logs)
        {
            Reset();
            foreach (var log in logs)
                Add(log);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}