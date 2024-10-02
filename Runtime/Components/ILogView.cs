using System.Collections.Generic;
using Again.Scripts.Runtime.Common;

namespace Again.Scripts.Runtime.Components
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