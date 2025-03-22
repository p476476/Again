using System.Threading.Tasks;
using Again.Runtime;
using Again.Runtime.ScriptImpoter;
using UnityEditor;
using UnityEngine;

namespace Plugins.Again.Editors
{
    public class SettingPanel : EditorWindow
    {
        private AgainSystemSetting _settings;
        
        private const string SettingsPath = "Assets/Settings/AgainSettings.asset";
        
        [MenuItem("Tools/Again/Setting")]
        public static void ShowWindow()
        {
            var window = GetWindow<SettingPanel>();
            window.LoadSettings();
        }
        
        private void OnDestroy()
        {
            SaveSettings();
        }

        private void OnGUI()
        {
            if (!_settings)
            {
                EditorGUILayout.HelpBox("Setting file not found. Please create one.", MessageType.Warning);
                if (GUILayout.Button("Create Setting File"))
                {
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    CreateSettingFile();
                }

                return;

            }
            
            _settings.useGoogleSheet = EditorGUILayout.Toggle("Use Google Sheet", _settings.useGoogleSheet);
            if (_settings.useGoogleSheet)
            {
                _settings.googleSheetId = EditorGUILayout.TextField("Google Sheet ID", _settings.googleSheetId);
                _settings.googleApiKey = EditorGUILayout.TextField("Google API Key", _settings.googleApiKey);
            }


            _settings.dialogueView = (GameObject)EditorGUILayout.ObjectField("Dialogue View", _settings.dialogueView, typeof(GameObject), false);
            _settings.logView = (GameObject)EditorGUILayout.ObjectField("Log View", _settings.logView, typeof(GameObject), false);
            _settings.optionMenuView = (GameObject)EditorGUILayout.ObjectField("Option Menu View", _settings.optionMenuView, typeof(GameObject), false);
            _settings.transferView = (GameObject)EditorGUILayout.ObjectField("Transfer View", _settings.transferView, typeof(GameObject), false);
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Download Sheet"))
            { 
                GoogleSheetDownloader.Download(_settings.googleSheetId, _settings.googleApiKey);
            }
        }

        private void LoadSettings()
        {
            _settings = AssetDatabase.LoadAssetAtPath<AgainSystemSetting>(SettingsPath);

            if (_settings == null)
            {
                Debug.LogWarning("Setting Data not found. Please create one.");
            }
        }

        private void SaveSettings()
        {
            if (_settings != null)
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }
        }

        private void CreateSettingFile()
        {
            _settings = CreateInstance<AgainSystemSetting>();
            AssetDatabase.CreateAsset(_settings, SettingsPath);
            
            _settings.dialogueView = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/Again/Prefabs/Components/Dialogue View.prefab");
            _settings.logView = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/Again/Prefabs/Components/Log View.prefab");
            _settings.optionMenuView = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/Again/Prefabs/Components/Option Menu View.prefab");
            _settings.transferView = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/Again/Prefabs/Components/Transfer View.prefab");
            
            AssetDatabase.SaveAssets();
        }

    }
}