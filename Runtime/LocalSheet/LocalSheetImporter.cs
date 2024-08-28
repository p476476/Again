using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Again.Runtime.GoogleSheet;
using Again.Scripts.Runtime.Commands;
using UnityEngine;

namespace Again.Scripts.Runtime.LocalSheet
{
    public class LocalSheetImporter : ISheetImporter
    {
        public async Task<List<string>> LoadScripts()
        {
            var files = Resources.LoadAll<TextAsset>("CSV");
            var scriptNames = new List<string>();
            foreach (var file in files) scriptNames.Add(file.name);

            return scriptNames;
        }

        public async Task<List<Command>> LoadScript(string scriptName)
        {
            var file = Resources.Load<TextAsset>($"CSV/{scriptName}");
            var lines = file.text.Split(",\"\"\n").ToList();
            var commands = ScriptSheetReader.Read(lines);

            return commands;
        }

        public async Task<Dictionary<string, List<string>>> LoadTransition()
        {
            return new Dictionary<string, List<string>>();
        }
    }
}