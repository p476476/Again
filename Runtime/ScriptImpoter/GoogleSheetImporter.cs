using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Again.Scripts.Runtime;
using Again.Scripts.Runtime.Commands;
using Again.Scripts.Runtime.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace Again.Runtime.GoogleSheet
{
    public class GoogleSheetImporter : ISheetImporter
    {
        private const string URLFormat =
            @"https://docs.google.com/spreadsheets/d/{0}/gviz/tq?tqx=out:csv&sheet={1}";

        private const string PageListSheetName = "Config";
        private const string TranslationSheetName = "Translation";
        private static readonly HttpClient client = new();
        private readonly string _sheetID;

        public GoogleSheetImporter([CanBeNull] string sheetID)
        {
            _sheetID = sheetID;
        }

        public async Task<List<string>> LoadScripts()
        {
            if (string.IsNullOrEmpty(_sheetID))
                return new List<string>();

            var url = string.Format(URLFormat, _sheetID, PageListSheetName);
            var data = await FetchData(url);

            var grid = _ParseCsv(data);

            var pageNames = grid.Select(row => row[0]).ToList();
            pageNames.RemoveAt(0);
            pageNames.RemoveAll(string.IsNullOrEmpty);

            return pageNames;
        }

        public async Task<List<Command>> LoadScript(string scriptName)
        {
            if (string.IsNullOrEmpty(_sheetID))
                Debug.LogError("No sheet ID provided");

            var url = string.Format(URLFormat, _sheetID, scriptName);
            var data = await FetchData(url);
            var lines = data.Split(",\"\"\n").ToList();
            lines.RemoveAt(0);
            var data2D = new List<List<string>>();
            foreach (var line in lines)
            {
                var rowString = line.Trim('"');
                data2D.Add(rowString.Split("\",\"").ToList());
            }

            var commands = ScriptSheetReader.Read(data2D);

            return commands;
        }

        public async Task<Dictionary<string, List<string>>> LoadTranslation()
        {
            if (string.IsNullOrEmpty(_sheetID))
                Debug.LogError("No sheet ID provided");

            var url = string.Format(URLFormat, _sheetID, TranslationSheetName);
            var data = await FetchData(url);
            var lines = data.Split(",\"\"\n").ToList();
            var languageCount = Enum.GetNames(typeof(Language)).Length;

            var dict = new Dictionary<string, List<string>>();
            foreach (var line in lines)
            {
                // 拆分資料
                var rowString = line.Substring(1, line.Length - 2);
                var values = rowString.Split("\",\"").ToList();
                dict[values[0]] = values.GetRange(2, languageCount).ToList();
            }

            return dict;
        }

        private List<List<string>> _ParseCsv(string csvStr)
        {
            var lines = csvStr.Split('\n');
            var result = new List<List<string>>();
            foreach (var line in lines)
            {
                var values = line.Split(',');
                for (var i = 0; i < values.Length; i++)
                    values[i] = values[i].Trim('"');
                result.Add(values.ToList());
            }

            return result;
        }

        private async Task<string> FetchData(string url)
        {
            try
            {
                // 發送 HTTP 請求並獲取響應
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // 讀取響應內容並指定使用 UTF-8 編碼
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                var responseString = Encoding.UTF8.GetString(responseBytes);

                return responseString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return null;
            }
        }
    }
}