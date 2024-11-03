using UnityEngine;

namespace Again.Runtime.Save.Structs
{
    public class CameraManagerSaveData
    {
        public Vector3 OriginalPosition;
        public TransformSaveData Transform;

        public static string ToJson(Vector3 originalPosition, Transform transform)
        {
            return JsonUtility.ToJson(new CameraManagerSaveData
            {
                OriginalPosition = originalPosition,
                Transform = new TransformSaveData(transform.localPosition, transform.rotation, transform.localScale)
            });
        }

        public static CameraManagerSaveData FromJson(string json)
        {
            return JsonUtility.FromJson<CameraManagerSaveData>(json);
        }
    }
}