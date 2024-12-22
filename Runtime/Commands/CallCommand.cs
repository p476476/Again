namespace Again.Runtime.Commands
{
    public class CallCommand : Command
    {
        public string ScriptName { get; set; } // 

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Execute()
        {
            AgainSystem.Instance.Execute(ScriptName);
        }
    }
}