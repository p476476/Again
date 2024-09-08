using System.Collections.Generic;

namespace Again.Scripts.Runtime.Commands
{
    public class EmitCommand : Command
    {
        public string Name { get; set; }
        public List<string> Parameters { get; set; }

        public override void Execute()
        {
            if (Parameters != null)
                AgainSystem.Instance.EventManager.Emit(Name, Parameters);
            else
                AgainSystem.Instance.EventManager.Emit(Name);

            AgainSystem.Instance.NextCommand();
        }
    }
}