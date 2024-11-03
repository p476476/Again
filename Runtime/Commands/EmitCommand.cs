using System.Collections.Generic;

namespace Again.Runtime.Commands
{
    public class EmitCommand : Command
    {
        public string Name { get; set; }
        public List<string> Parameters { get; set; }

        public override void Execute()
        {
            if (Parameters.Count > 0)
                AgainSystem.Instance.EventManager.Emit(Name, Parameters);
            else
                AgainSystem.Instance.EventManager.Emit(Name);

            AgainSystem.Instance.NextCommand();
        }
    }
}