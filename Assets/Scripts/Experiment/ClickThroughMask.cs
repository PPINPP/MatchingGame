using UnityEngine;
using UnityEngine.EventSystems;

public class ClickThroughMask : MonoBehaviour, ICanvasRaycastFilter
{
    [SerializeField] private RectTransform maskArea;

    // This method determines if a raycast should block interaction.
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (maskArea == null) return true;

        // Convert screen point to local point in mask area
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskArea, sp, eventCamera, out Vector2 localPoint);

        // Return true if outside the mask area, allowing interaction
        return !maskArea.rect.Contains(localPoint);
    }
}
