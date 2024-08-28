using System.Collections.Generic;
using System.Threading.Tasks;
using Again.Scripts.Runtime.Commands;

namespace Again.Scripts.Runtime
{
    public interface ISheetImporter
    {
        public Task<List<string>> LoadScripts();
        public Task<List<Command>> LoadScript(string scriptName);
        public Task<Dictionary<string, List<string>>> LoadTransition();
    }
}