using Again.Runtime;
using UnityEngine;

namespace Again._Examples.Scripts
{
    public class HelloWorld : MonoBehaviour
    {
        private void Start()
        {
            AgainSystem.Instance.Execute("HelloWorld");
        }
    }
}