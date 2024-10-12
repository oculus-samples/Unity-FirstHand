// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ConditionalGameObjectLayer : ActiveStateObserver
    {
        [SerializeField, Layer]
        private int _activeLayer;

        private int _originalLayer;

        protected override void Start()
        {
            _originalLayer = gameObject.layer;
            base.Start();
            HandleActiveStateChanged();
        }

        protected override void HandleActiveStateChanged() => SetLayerRecursive(transform, Active ? _activeLayer : _originalLayer);

        private void SetLayerRecursive(Transform root, int layer)
        {
            if (root.TryGetComponent<ConditionalGameObjectLayer>(out var l) && l != this) return;

            root.gameObject.layer = layer;

            foreach (Transform child in root)
                SetLayerRecursive(child, layer);
        }

        public class LayerAttribute : PropertyAttribute { }

#if UNITY_EDITOR
        [UnityEditor.CustomPropertyDrawer(typeof(LayerAttribute))]
        public class LayerAttributeDrawer : UnityEditor.PropertyDrawer
        {
            public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
            {
                UnityEditor.EditorGUI.BeginChangeCheck();
                int newValue = UnityEditor.EditorGUI.LayerField(position, label, property.intValue);
                if (UnityEditor.EditorGUI.EndChangeCheck())
                {
                    property.intValue = newValue;
                }
            }
        }
#endif
    }
}
