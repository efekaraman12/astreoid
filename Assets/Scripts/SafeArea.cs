using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect safeArea;
    private Vector2 minAnchor;
    private Vector2 maxAnchor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateSafeArea();
    }

    private void Start()
    {
        UpdateSafeArea();
    }

    private void UpdateSafeArea()
    {
        if (rectTransform == null) return;

        safeArea = Screen.safeArea;

        minAnchor = safeArea.position;
        maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
    }

    private void OnRectTransformDimensionsChange()
    {
        if (gameObject.activeInHierarchy)
        {
            UpdateSafeArea();
        }
    }
}