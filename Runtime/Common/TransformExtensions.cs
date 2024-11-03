using UnityEngine;

namespace Again.Runtime.Common
{
    public static class TransformExtensions
    {
        public static void DestroyChildren(this Transform transform)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
                Object.Destroy(transform.GetChild(i).gameObject);
        }

        public static void ResetAndHide(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.gameObject.SetActive(false);
        }
    }
}