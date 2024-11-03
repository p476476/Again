using System.Collections.Generic;
using Again.Runtime.Components.Structs;

namespace Again.Runtime.Components.Interfaces
{
    public interface ILogView
    {
        public void Add(DialogueLog log)
        {
        }

        public void SetLogs(List<DialogueLog> logs)
        {
        }

        public void Reset()
        {
        }
    }
}