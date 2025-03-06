using System.Collections.Generic;
using System.Threading.Tasks;
using Again.Runtime.Commands;

namespace Again.Runtime.ScriptImpoter
{
    public interface ISheetImporter
    {
        public Task<List<string>> LoadScripts();
        public Task<List<Command>> LoadScript(string scriptName);
        public Task<Dictionary<string, List<string>>> LoadTranslation();
        public Task<Dictionary<string, SpineInfo>> LoadSpineSetting();
    }
}