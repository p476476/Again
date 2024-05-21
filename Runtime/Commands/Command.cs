namespace Again.Scripts.Runtime.Commands
{
    public abstract class Command
    {
        public int Id { get; set; }
        public abstract void Execute();
    }
}