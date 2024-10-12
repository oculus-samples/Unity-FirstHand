// Copyright (c) Meta Platforms, Inc. and affiliates.

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Can set a start value to a StringPropertyBehaviour
    /// </summary>
    public class StringPropertyBehaviour : PropertyBehaviour<string>, ISerializationCallbackReceiver
    {
        public string startValue = "";

        private void Awake()
        {
            if (string.IsNullOrEmpty(Value))
            {
                _value = startValue;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(Value))
            {
                _value = startValue;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StringPropertyBehaviour), true), CanEditMultipleObjects]
    class StringPropertyBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!Application.isPlaying) return;

            var targets = serializedObject.targetObjects;
            var value = GetValue(targets[0]);
            for (int i = 1; i < targets.Length; i++)
            {
                if (GetValue(targets[i]) != value)
                {
                    value = "-";
                    break;
                }
            }

            GUI.color = new Color(1, 1, 1, 0.5f);
            var newValue = EditorGUILayout.DelayedTextField("Value", value);
            if (newValue != value)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    SetValue(targets[i], newValue);
                }
            }

            string GetValue(UnityEngine.Object obj) => (obj as StringPropertyBehaviour).Value;
            void SetValue(UnityEngine.Object obj, string newValue) => (obj as StringPropertyBehaviour).Value = newValue;
        }
    }
#endif
}
