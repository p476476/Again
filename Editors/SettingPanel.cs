using System;
using System.Linq;
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
        private Tab _selectedTab = Tab.General;
        private const string SettingsPath = "Assets/Settings/AgainSettings.asset";
        
        private enum Tab
        {
            General,
            Spine
        }
        
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
            GUILayout.BeginHorizontal();
            foreach (Tab tab in Enum.GetValues(typeof(Tab)))
            {
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                if (_selectedTab == tab)
                {
                    buttonStyle.normal.background = MakeTex(2, 2, new Color(0.0f, 0.5f, 0.9f, 1.0f));
                }
                
                if (GUILayout.Button(tab.ToString(), buttonStyle))
                {
                    _selectedTab = tab;
                }
            }
            GUILayout.EndHorizontal();

            switch (_selectedTab)
            {
                case Tab.General:
                    ShowGeneralSettingPage();
                    break;
                case Tab.Spine:
                    ShowSpineSettingPage();
                    break;
            }
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private Vector2 _scrollPosition;
        private void ShowSpineSettingPage()
        {
            EditorGUILayout.HelpBox("Spine Setting Page", MessageType.Info);
            
            var spineInfos = _settings.spineInfos;
            
            if (spineInfos == null)
            {
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            for (int i = 0; i < spineInfos.Length; i++)
            {
                var spineInfo = spineInfos[i];
                spineInfo.Name = EditorGUILayout.TextField("Name", spineInfo.Name);
                EditorGUILayout.BeginHorizontal();
                Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                float labelWidth = 60f;
                float fieldWidth = (rect.width - labelWidth * 4) / 4;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, rect.height), "Offset X");
                spineInfo.OffsetX = EditorGUI.FloatField(new Rect(rect.x + labelWidth, rect.y, fieldWidth, rect.height), spineInfo.OffsetX);

                EditorGUI.LabelField(new Rect(rect.x + labelWidth + fieldWidth, rect.y, labelWidth, rect.height), "Offset Y");
                spineInfo.OffsetY = EditorGUI.FloatField(new Rect(rect.x + labelWidth * 2 + fieldWidth, rect.y, fieldWidth, rect.height), spineInfo.OffsetY);

                EditorGUI.LabelField(new Rect(rect.x + labelWidth * 2 + fieldWidth * 2, rect.y, labelWidth, rect.height), "Scale X");
                spineInfo.ScaleX = EditorGUI.FloatField(new Rect(rect.x + labelWidth * 3 + fieldWidth * 2, rect.y, fieldWidth, rect.height), spineInfo.ScaleX);

                EditorGUI.LabelField(new Rect(rect.x + labelWidth * 3 + fieldWidth * 3, rect.y, labelWidth, rect.height), "Scale Y");
                spineInfo.ScaleY = EditorGUI.FloatField(new Rect(rect.x + labelWidth * 4 + fieldWidth * 3, rect.y, fieldWidth, rect.height), spineInfo.ScaleY);

                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Remove"))
                {
                    var list = spineInfos.ToList();
                    list.RemoveAt(i);
                    _settings.spineInfos = list.ToArray();
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add"))
            {
                Array.Resize(ref _settings.spineInfos, spineInfos.Length + 1);
                _settings.spineInfos[^1] = new SpineInfo("New Spine", 0, 0, 1, 1);
            }
        }

        private void ShowGeneralSettingPage()
        {
            if (!_settings)
            {
                EditorGUILayout.HelpBox("Setting file not found. Please create one.", MessageType.Warning);
                if (GUILayout.Button("建立設定檔"))
                {
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    CreateSettingFile();
                }

                return;

            }
            
            var useRemoteContent = new GUIContent("Use Google Sheet", "要用遠端的腳本的話，請打勾。");
            _settings.useGoogleSheet = EditorGUILayout.Toggle(useRemoteContent, _settings.useGoogleSheet);
            if (_settings.useGoogleSheet)
            {
                var googleSheetContent = new GUIContent("Google Sheet ID", "URL 中的 Google Sheet ID");
                var googleApiKeyContent = new GUIContent("Google API Key", "請求分頁資料用的 API Key");
                _settings.googleSheetId = EditorGUILayout.TextField(googleSheetContent, _settings.googleSheetId);
                _settings.googleApiKey = EditorGUILayout.TextField(googleApiKeyContent, _settings.googleApiKey);
            }


            var dialogueViewContent = new GUIContent("Dialogue View", "對話框的預置物件");
            var logViewContent = new GUIContent("Log View", "對話紀錄的預置物件");
            var optionMenuViewContent = new GUIContent("Option Menu View", "選項選單的預置物件");
            var transferViewContent = new GUIContent("Transfer View", "場景轉換的預置物件");
            _settings.dialogueView = (GameObject)EditorGUILayout.ObjectField(dialogueViewContent, _settings.dialogueView, typeof(GameObject), false);
            _settings.logView = (GameObject)EditorGUILayout.ObjectField(logViewContent, _settings.logView, typeof(GameObject), false);
            _settings.optionMenuView = (GameObject)EditorGUILayout.ObjectField(optionMenuViewContent, _settings.optionMenuView, typeof(GameObject), false);
            _settings.transferView = (GameObject)EditorGUILayout.ObjectField(transferViewContent, _settings.transferView, typeof(GameObject), false);
            
            EditorGUILayout.Space();
            if (GUILayout.Button("下載遠端腳本"))
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
            _settings.spineInfos = Array.Empty<SpineInfo>();
            
            AssetDatabase.SaveAssets();
        }

    }
}