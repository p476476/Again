namespace Again.Runtime.Commands
{
    public abstract class Command
    {
        public int Id { get; set; }
        public bool IsSkip { get; set; }
        public abstract void Execute();
    }
}