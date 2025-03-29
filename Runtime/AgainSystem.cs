using System.Collections.Generic;
using System.IO;
using System.Linq;
using Again.Runtime.Commands;
using Again.Runtime.Components.Interfaces;
using Again.Runtime.Components.Managers;
using Again.Runtime.Enums;
using Again.Runtime.Save.Structs;
using Again.Runtime.ScriptImpoter;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Again.Runtime
{
    public class AgainSystem : MonoBehaviour
    {
        private AgainSystemSetting _setting;
        [SerializeField] private Transform uiCanvas;

        private List<Command> _commands;
        private int _currentCommandIndex = -1;
        private bool _isAutoNext;
        private bool _isPause;
        private bool _isSkip;

        public UnityEvent OnScriptFinished { get; } = new();
        public ITransferView TransferView { get; private set; }
        public ISheetImporter SheetImporter { get; private set; }
        public DialogueManager DialogueManager { get; private set; }
        public SpineManager SpineManager { get; private set; }
        public ImageManager ImageManager { get; private set; }
        public CameraManager CameraManager { get; private set; }
        public EventManager EventManager { get; private set; }
        public AudioManager AudioManager { get; }

        public static AgainSystem Instance { get; private set; }

        private async void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(transform.parent.gameObject);
            
            _setting = AssetDatabase.LoadAssetAtPath<AgainSystemSetting>("Assets/Settings/AgainSettings.asset");
            
            _commands = new List<Command>();
            EventManager = new EventManager();
            DialogueManager = GetComponent<DialogueManager>();
            SpineManager = GetComponent<SpineManager>();
            CameraManager = GetComponent<CameraManager>();
            ImageManager = GetComponent<ImageManager>();
            TransferView = Instantiate(_setting.transferView, uiCanvas).GetComponent<ITransferView>();
            DialogueManager.Init(uiCanvas, _setting);

            if (_setting.useGoogleSheet)
                SheetImporter = new GoogleSheetImporter(_setting.googleSheetId, _setting.googleApiKey);
            else
                SheetImporter = new LocalSheetImporter();
            DialogueManager.SetLocaleDict(await SheetImporter.LoadTranslation());
            SpineManager.UpdateSpineInfos(_setting.spineInfos.ToDictionary(info => info.Name, info => info));
        }

        public async void Execute(string scriptName)
        {
            var commands = await SheetImporter.LoadScript(scriptName);
            Stop();
            CameraManager.Show();
            RunCommands(commands);
        }

        public void RunCommands(List<Command> commands)
        {
            if (commands.Count == 0)
            {
                Debug.Log("腳本沒有任何指令");
                OnScriptFinished?.Invoke();
                return;
            }

            _commands = commands;
            _currentCommandIndex = -1;
            NextCommand();
        }

        public void NextCommand()
        {
            if (_isPause)
            {
                Debug.Log("AgainSystem 暫停中");
                return;
            }

            _currentCommandIndex++;
            if (_currentCommandIndex < _commands.Count)
            {
                _commands[_currentCommandIndex].IsSkip = _isSkip;
                _commands[_currentCommandIndex].Execute();

                var nextCommand = _currentCommandIndex + 1 < _commands.Count
                    ? _commands[_currentCommandIndex + 1]
                    : null;
                if (nextCommand != null && nextCommand.IsJoin) NextCommand();
                return;
            }

            CameraManager.Reset();
            SpineManager.Reset();
            ImageManager.Reset();
            DialogueManager.Reset();
            DialogueManager.Hide();
            OnScriptFinished?.Invoke();
        }

        public void GoToCommand(Command command)
        {
            _currentCommandIndex = _commands.IndexOf(command) - 1;
            NextCommand();
        }

        public void Stop()
        {
            CameraManager.Reset();
            SpineManager.Reset();
            ImageManager.Reset();
            DialogueManager.Reset();
            DialogueManager.Hide();

            _currentCommandIndex = -1;
            _commands.Clear();
            _isPause = false;
            _isSkip = false;
        }

        public async void ReloadTranslation()
        {
            DialogueManager.SetLocaleDict(await SheetImporter.LoadTranslation());
        }

        public void SetLanguage(Language language)
        {
            DialogueManager.SetLanguage(language);
        }

        public void SetLocaleDict(Dictionary<string, List<string>> dict)
        {
            DialogueManager.SetLocaleDict(dict);
        }

        public void SetPause(bool isPause)
        {
            _isPause = isPause;
        }

        public void SetSkip(bool isSkip)
        {
            _isSkip = isSkip;
        }

        public bool GetSkip()
        {
            return _isSkip;
        }

        public void SetAutoNext(bool isAutoNext)
        {
            _isAutoNext = isAutoNext;
        }

        public bool GetAutoNext()
        {
            return _isAutoNext;
        }

        [ContextMenu("Save")]
        public void Save()
        {
            var saveData = new AgainSystemSaveData
            {
                cameraManagerSaveData = CameraManager.Save(),
                imageManagerSaveData = ImageManager.Save(),
                spineManagerSaveData = SpineManager.Save()
            };
            var str = JsonUtility.ToJson(saveData);
            Debug.Log(str);
            var path = Application.persistentDataPath + "/save.txt";
            File.WriteAllText(path, str);
        }

        [ContextMenu("Load")]
        public void Load()
        {
            var path = Application.persistentDataPath + "/save.txt";
            var str = File.ReadAllText(path);
            ImageManager.Load(str);
        }
    }
}