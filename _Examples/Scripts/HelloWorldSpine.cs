using Again.Runtime;
using UnityEngine;

namespace Again._Examples.Scripts
{
    public class HelloWorldSpine : MonoBehaviour
    {
        private void Start()
        {
            AgainSystem.Instance.Execute("測試範例");
        }
    }
}