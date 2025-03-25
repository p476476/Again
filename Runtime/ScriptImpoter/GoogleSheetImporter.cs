using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Again.Runtime.Commands;
using Again.Runtime.Enums;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace Again.Runtime.ScriptImpoter
{
    public class GoogleSheetImporter : ISheetImporter
    {
        private const string URLFormat =
            @"https://docs.google.com/spreadsheets/d/{0}/gviz/tq?tqx=out:csv&sheet={1}";

        private const string TranslationSheetName = "Translation";
        private const string SpineSettingSheetName = "SpineSetting";

        private static readonly HttpClient client = new();
        private readonly string _apiKey;
        private readonly string _sheetID;

        public GoogleSheetImporter(string sheetID, string apiKey)
        {
            _apiKey = apiKey;
            _sheetID = sheetID;
        }

        public async Task<List<string>> LoadScripts()
        {
            if (string.IsNullOrEmpty(_sheetID))
                return new List<string>();

            var url = $"https://sheets.googleapis.com/v4/spreadsheets/{_sheetID}?key={_apiKey}";

            var data = await FetchData(url);

            var json = JObject.Parse(data);

            var sheetNames = new List<string>();
            foreach (var sheet in json["sheets"]!)
                sheetNames.Add(sheet["properties"]["title"].ToString());

            return sheetNames;
        }

        public async Task<List<Command>> LoadScript(string scriptName)
        {
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


        public async Task<Dictionary<string, SpineInfo>> LoadSpineSetting()
        {
            var url = string.Format(URLFormat, _sheetID, SpineSettingSheetName);
            var data = await FetchData(url);
            var lines = data.Split(",\"\"\n").ToList();
            lines.RemoveAt(0); // 移除標題列

            var dict = new Dictionary<string, SpineInfo>();
            foreach (var line in lines)
            {
                var values = line.Split("\",\"").ToList();
                if (values.Count < 2) continue;
                if (!float.TryParse(values[1], out var x)) x = 0;
                if (!float.TryParse(values[2], out var y)) y = 0;
                if (!float.TryParse(values[3], out var sx)) sx = 1;
                if (!float.TryParse(values[4], out var sy)) sy = 1;
                dict[values[0]] = new SpineInfo(values[0], x, y, sx, sy);
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