namespace Again.Scripts.Runtime.Common
{
    public struct DialogueLog
    {
        public string CharacterKey;
        public string TextKey;

        public DialogueLog(string characterKey, string textKey)
        {
            CharacterKey = characterKey;
            TextKey = textKey;
        }
    }
}