using System.Collections.Generic;
using Again.Runtime.Common;
using Again.Runtime.Components.Interfaces;
using Again.Runtime.Components.Structs;
using UnityEngine;

namespace Again.Runtime.Components.Views
{
    public class LogView : MonoBehaviour, ILogView
    {
        public Transform logContainer;
        public GameObject logPrefab;
        public List<LogBlock> logs;

        private void Awake()
        {
            transform.ResetAndHide();
            AgainSystem.Instance.EventManager.On("ShowLog", Show);
        }

        private void OnDestroy()
        {
            if (AgainSystem.Instance == null) return;
            AgainSystem.Instance.EventManager.Off("ShowLog", Show);
        }

        public void Reset()
        {
            foreach (var log in logs)
                Destroy(log.gameObject);
            logs.Clear();
        }

        public virtual void Add(DialogueLog log)
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