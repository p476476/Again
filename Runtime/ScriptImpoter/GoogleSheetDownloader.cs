using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace Again.Runtime.ScriptImpoter
{
    public class GoogleSheetDownloader
    {
        private static readonly HttpClient client = new();

        public static void Download(string sheetID, string apiKey)
        {
            var scripts =  LoadScripts(sheetID, apiKey);
            var URLFormat =
                @"https://docs.google.com/spreadsheets/d/{0}/gviz/tq?tqx=out:csv&sheet={1}";
            foreach (var script in scripts)
            {
                var url = string.Format(URLFormat, sheetID, script);
                var data = FetchData(url);
                var path = $"Assets/Resources/CSV/{script}.csv";
                System.IO.File.WriteAllTextAsync(path, data);
            }
        }

        private static  List<string> LoadScripts(string sheetID, string apiKey)
        {
            if (string.IsNullOrEmpty(sheetID) || string.IsNullOrEmpty(apiKey))
                return new List<string>();

            var url = $"https://sheets.googleapis.com/v4/spreadsheets/{sheetID}?key={apiKey}";

            var data = FetchData(url);

            var json = JObject.Parse(data);

            var sheetNames = new List<string>();
            foreach (var sheet in json["sheets"]!)
                sheetNames.Add(sheet["properties"]["title"].ToString());

            return sheetNames;
        }


        private static string FetchData(string url)
        {
            try
            {
                // 發送 HTTP 請求並獲取響應
                var response =  client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                // 讀取響應內容並指定使用 UTF-8 編碼
                var responseBytes = response.Content.ReadAsByteArrayAsync().Result;
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