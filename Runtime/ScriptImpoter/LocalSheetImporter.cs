using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Again.Runtime.Commands;
using UnityEngine;

namespace Again.Runtime.ScriptImpoter
{
    public class LocalSheetImporter : ISheetImporter
    {
        private readonly List<string> _ignoreFiles = new() { "Translation" };

        public async Task<List<string>> LoadScripts()
        {
            var files = Resources.LoadAll<TextAsset>("TSV");
            var scriptNames = new List<string>();
            foreach (var file in files)
            {
                if (_ignoreFiles.Contains(file.name)) continue;
                scriptNames.Add(file.name);
            }

            return scriptNames;
        }

        public async Task<List<Command>> LoadScript(string scriptName)
        {
            var file = Resources.Load<TextAsset>($"TSV/{scriptName}");
            var lines = file.text.Split(Environment.NewLine).ToList();
            lines.RemoveAt(0);
            var data = new List<List<string>>();
            foreach (var line in lines) data.Add(line.Split("\t").ToList());
            var commands = ScriptSheetReader.Read(data);

            return commands;
        }

        public async Task<Dictionary<string, List<string>>> LoadTranslation()
        {
            var file = Resources.Load<TextAsset>("TSV/Translation");
            if (file == null) return new Dictionary<string, List<string>>();
            var lines = file.text.Split("\r\n").ToList();
            var dict = new Dictionary<string, List<string>>();
            for (var i = 1; i < lines.Count; i++)
            {
                var values = lines[i].Split("\t").ToList();
                if (values.Count < 2) continue;
                dict[values[0]] = values.GetRange(2, values.Count - 2).ToList();
            }

            return dict;
        }
    }
}