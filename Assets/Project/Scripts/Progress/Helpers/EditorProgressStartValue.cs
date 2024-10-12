// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sets a progress value when the app starts
    /// </summary>
    public class EditorProgressStartValue : MonoBehaviour
    {
        public IntValueAsset startValue;

#if UNITY_EDITOR
        private void Awake() => GetComponent<ProgressTracker>().SetProgress(startValue);

        [UnityEditor.CustomEditor(typeof(EditorProgressStartValue))]
        class EditorProgressStartValueEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                var prop = serializedObject.FindProperty(nameof(startValue));
                if (prop.objectReferenceValue != null)
                {
                    var tmpEditor = CreateEditor(prop.objectReferenceValue);
                    tmpEditor.OnInspectorGUI();
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }
        }
#endif
    }
}
