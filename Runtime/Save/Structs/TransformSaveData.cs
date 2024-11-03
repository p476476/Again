using System;
using UnityEngine;

namespace Again.Runtime.Save.Structs
{
    [Serializable]
    public struct TransformSaveData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformSaveData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}