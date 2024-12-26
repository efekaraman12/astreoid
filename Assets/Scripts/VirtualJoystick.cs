using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Terresquall
{
    public enum JoystickType
    {
        Movement,
        Fire
    }

    [RequireComponent(typeof(RectTransform))]
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;

        [Header("Settings")]
        [SerializeField] private JoystickType joystickType = JoystickType.Movement;
        [SerializeField] private bool isDynamicJoystick = true;
        [SerializeField] private float moveThreshold = 1f;
        [Range(0f, 1f)] public float deadzone = 0.1f;

        private Canvas canvas;
        private Camera cam;
        private Vector2 input = Vector2.zero;
        private bool isPressed;
        private Vector2 defaultPosition;

        public Vector2 Direction => input;
        public bool IsPressed => isPressed;
        public JoystickType Type => joystickType;
        public float MoveThreshold { get => moveThreshold; set => moveThreshold = Mathf.Abs(value); }

        private void Start()
        {
            canvas = GetComponentInParent<Canvas>();
            cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
            defaultPosition = background.anchoredPosition;

            // Dinamik joystick başlangıçta gizli, ateş butonu görünür
            if (isDynamicJoystick)
            {
                background.gameObject.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isDynamicJoystick)
            {
                // Ekranın sol yarısında mı kontrol et
                if (eventData.position.x > Screen.width * 0.5f) return;

                background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
                background.gameObject.SetActive(true);
            }

            isPressed = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isPressed) return;

            Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
            Vector2 radius = background.sizeDelta / 2;
            input = ((Vector2)eventData.position - position) / (radius * canvas.scaleFactor);

            if (input.magnitude > 1f)
                input = input.normalized;

            // Deadzone kontrolü
            if (input.magnitude < deadzone)
                input = Vector2.zero;

            // Handle pozisyonunu güncelle
            Vector2 handlePosition = input * radius;
            handle.anchoredPosition = handlePosition;

            // Dinamik joystick için hareket eşiği kontrolü
            if (isDynamicJoystick && input.magnitude > moveThreshold)
            {
                Vector2 difference = input * ((input.magnitude - moveThreshold) * radius);
                background.anchoredPosition += difference;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;

            if (isDynamicJoystick)
            {
                background.gameObject.SetActive(false);
                background.anchoredPosition = defaultPosition;
            }
        }

        private Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            Vector2 localPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                cam,
                out localPoint);
            return localPoint;
        }
    }
}
