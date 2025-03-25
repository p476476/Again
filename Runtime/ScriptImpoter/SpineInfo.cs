using UnityEngine;
using UnityEngine.Serialization;

namespace Again.Runtime.ScriptImpoter
{
    [System.Serializable]
    public class SpineInfo
    {
        public string Name;
        public float OffsetX;
        public float OffsetY;
        public float ScaleX;
        public float ScaleY;

        public SpineInfo(string name, float offsetX, float offsetY, float scaleX, float scaleY)
        {
            this.Name = name;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
        }
    }
}