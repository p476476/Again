using UnityEngine;

namespace Again.Scripts.Runtime.Examples
{
    public class HelloWorldSpine : MonoBehaviour
    {
        private void Start()
        {
            AgainSystem.Instance.Execute("測試範例");
        }
    }
}