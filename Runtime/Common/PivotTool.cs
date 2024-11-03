using UnityEngine;

namespace Again.Runtime.Common
{
    public static class PivotTool
    {
        // Set the RectTransform's pivot point in world coordinates, without moving the position.
        // This is like dragging the pivot handle in the editor.
        //
        public static void SetPivotInWorldSpace(RectTransform rectTransform, Vector2 pivot)
        {
            Vector3 deltaPosition = rectTransform.pivot - pivot; // get change in pivot
            deltaPosition.Scale(rectTransform.rect.size); // apply sizing
            deltaPosition.Scale(rectTransform.localScale); // apply scaling
            deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

            rectTransform.pivot = pivot; // change the pivot
            rectTransform.localPosition -= deltaPosition; // reverse the position change
        }
    }
}