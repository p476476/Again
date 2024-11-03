namespace Again.Runtime.ScriptImpoter
{
    public struct PropertyInfo
    {
        public string Name;
        public string Type;
        public bool CanBeEmpty;

        public PropertyInfo(string name, string type, bool canBeEmpty)
        {
            Name = name;
            Type = type;
            CanBeEmpty = canBeEmpty;
        }
    }
}