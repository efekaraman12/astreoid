using UnityEngine;
using UnityEditor;

namespace Terresquall
{
    [CustomEditor(typeof(VirtualJoystick))]
    [CanEditMultipleObjects]
    public class VirtualJoystickEditor : Editor
    {
        private VirtualJoystick joystick;
        private RectTransform rectTransform;
        private Canvas canvas;

        private void OnEnable()
        {
            joystick = (VirtualJoystick)target;
            rectTransform = joystick.GetComponent<RectTransform>();
            canvas = joystick.GetComponentInParent<Canvas>();
        }

        public override void OnInspectorGUI()
        {
            if (!canvas && !EditorUtility.IsPersistent(target))
            {
                EditorGUILayout.HelpBox("Bu GameObject bir Canvas'a bağlı olmalıdır!", MessageType.Warning);
            }

            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (joystick == null) return;

            Handles.BeginGUI();

            // Joystick alanını görselleştir
            if (joystick.gameObject.activeInHierarchy)
            {
                // Joystick çemberi
                Handles.color = new Color(0, 1, 0, 0.1f);
                float radius = rectTransform.sizeDelta.x / 2;
                Handles.DrawSolidArc(joystick.transform.position, Vector3.forward, Vector3.right, 360, radius);

                // Deadzone çemberi
                Handles.color = new Color(1, 0, 0, 0.2f);
                Handles.DrawSolidArc(joystick.transform.position, Vector3.forward, Vector3.right, 360, radius * joystick.deadzone);
            }

            Handles.EndGUI();
        }
    }
}