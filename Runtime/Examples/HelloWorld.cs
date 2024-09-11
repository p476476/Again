using UnityEngine;

namespace Again.Scripts.Runtime.Examples
{
    public class HelloWorld : MonoBehaviour
    {
        private void Start()
        {
            AgainSystem.Instance.Execute("HelloWorld");
        }
    }
}