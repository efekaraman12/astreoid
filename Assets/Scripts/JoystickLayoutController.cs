using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class JoystickLayoutController : MonoBehaviour
{
    [SerializeField] private float minSize = 200f;
    [SerializeField] private float maxSize = 400f;
    [SerializeField] private float screenWidthRatio = 0.25f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateSize();
    }

    private void UpdateSize()
    {
        if (rectTransform == null) return;

        float screenWidth = Screen.width;
        float targetSize = screenWidth * screenWidthRatio;
        targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetSize);
    }

    private void OnRectTransformDimensionsChange()
    {
        if (gameObject.activeInHierarchy)
        {
            UpdateSize();
        }
    }
}