using UnityEngine;

namespace Again.Runtime
{
    [CreateAssetMenu(fileName = "AgainSystem", menuName = "Again/AgainSystemSetting")]
    public class AgainSystemSetting : ScriptableObject
    {
        public bool useGoogleSheet;
        public string googleSheetId = "18vCXwuMSR7K0FUEEcHy7X5NzgtyWyUgr7sbeIRW74zs";
        public string googleApiKey = "";
        public GameObject dialogueView;
        public GameObject logView;
        public GameObject optionMenuView;
        public GameObject transferView;
    }
}