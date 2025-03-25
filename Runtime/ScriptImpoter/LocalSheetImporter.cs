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
        private readonly List<string> _ignoreFiles = new() { "Translation", "SpineSetting" };

        public Task<List<string>> LoadScripts()
        {
            var files = Resources.LoadAll<TextAsset>("CSV");
            var scriptNames = new List<string>();
            foreach (var file in files)
            {
                if (_ignoreFiles.Contains(file.name)) continue;
                scriptNames.Add(file.name);
            }

            return Task.FromResult(scriptNames);
        }

        public Task<List<Command>> LoadScript(string scriptName)
        {
            var file = Resources.Load<TextAsset>($"CSV/{scriptName}");
            var lines = file.text.Split(",\"\"\n").ToList();
            lines.RemoveAt(0);
            var data2D = new List<List<string>>();
            foreach (var line in lines)
            {
                var rowString = line.Trim('"');
                data2D.Add(rowString.Split("\",\"").ToList());
            }
            var commands = ScriptSheetReader.Read(data2D);

            return Task.FromResult(commands);
        }

        public Task<Dictionary<string, List<string>>> LoadTranslation()
        {
            var file = Resources.Load<TextAsset>("CSV/Translation");
            if (file == null) return Task.FromResult(new Dictionary<string, List<string>>());
            var lines = file.text.Split("\r\n").ToList();
            var dict = new Dictionary<string, List<string>>();
            for (var i = 1; i < lines.Count; i++)
            {
                var values = lines[i].Split("\t").ToList();
                if (values.Count < 2) continue;
                dict[values[0]] = values.GetRange(2, values.Count - 2).ToList();
            }

            return Task.FromResult(dict);
        }

        public Task<Dictionary<string, SpineInfo>> LoadSpineSetting()
        {
            var file = Resources.Load<TextAsset>("CSV/SpineSetting");
            if (file == null) return Task.FromResult(new Dictionary<string, SpineInfo>());
            var lines = file.text.Split("\r\n").ToList();
            var dict = new Dictionary<string, SpineInfo>();

            for (var i = 1; i < lines.Count; i++)
            {
                var values = lines[i].Split("\t").ToList();
                if (values.Count < 2) continue;
                if (!float.TryParse(values[1], out var x)) x = 0;
                if (!float.TryParse(values[2], out var y)) y = 0;
                if (!float.TryParse(values[3], out var sx)) sx = 1;
                if (!float.TryParse(values[4], out var sy)) sy = 1;
                dict[values[0]] = new SpineInfo(values[0], x, y, sx, sy);
            }

            return Task.FromResult(dict);
        }
    }
}