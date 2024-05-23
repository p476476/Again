using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Again.Scripts.Runtime;
using Again.Scripts.Runtime.Enums;
using UnityEngine;

namespace Again.Runtime.GoogleSheet
{
    public class GoogleSheetImporter : MonoBehaviour
    {
        private const string URLFormat = @"https://docs.google.com/spreadsheets/d/{0}/gviz/tq?tqx=out:csv&sheet={1}";
        private const string PageListSheetName = "Config";
        private const string TranslationSheetName = "Translation";
        public string sheetID = "1dNdvKzT7IZryEIou0suLthyALAKxjj1Tf9NcbFznWzs";

        public SheetSelectView sheetSelectView;

        private async void Start()
        {
            await _GetScriptPages();
            await _ImportTranslationPage();
        }


        public async Task ImportPage(string pageName)
        {
            var url = string.Format(URLFormat, sheetID, pageName);
            var data = await FetchData(url);
            var lines = data.Split(",\"\"\n").ToList();
            var commands = ScriptSheetReader.Read(lines);
            AgainSystem.Instance.RunCommands(commands);
        }


        private async Task _ImportTranslationPage()
        {
            var url = string.Format(URLFormat, sheetID, TranslationSheetName);
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

            AgainSystem.Instance.DialogueManager.SetLocaleDict(dict);
        }

        public async Task Reload()
        {
            await _GetScriptPages();
            await _ImportTranslationPage();
        }

        private async Task _GetScriptPages()
        {
            var url = string.Format(URLFormat, sheetID, PageListSheetName);
            var data = await FetchData(url);

            var grid = _ParseCsv(data);

            var pageNames = grid.Select(row => row[0]).ToList();
            pageNames.RemoveAt(0);
            pageNames.RemoveAll(string.IsNullOrEmpty);

            sheetSelectView.SetPages(pageNames);
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
            var webClient = new WebClient();
            var request = default(Task<string>);
            try
            {
                request = webClient.DownloadStringTaskAsync(url);
            }
            catch (WebException)
            {
                var message = $"Bad URL '{url}'";
                throw new Exception(message);
            }

            while (!request.IsCompleted)
                await Task.Delay(100);

            if (request.IsFaulted)
            {
                var message = $"Bad URL '{url}'";
                throw new Exception(message);
            }

            return request.Result;
        }
    }
}