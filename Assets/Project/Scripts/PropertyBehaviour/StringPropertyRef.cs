// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Serialized reference to an IProperty of string type
    /// </summary>
    [Serializable]
    struct StringPropertyRef : IProperty<string>, ISerializationCallbackReceiver
    {
        [SerializeField, Interface(typeof(IProperty<string>))]
        private MonoBehaviour _property;
        public IProperty<string> Property;

        public string Value { get => Property.Value; set => Property.Value = value; }

        public event Action WhenChanged
        {
            add => Property.WhenChanged += value;
            remove
            {
                if (Property != null) { Property.WhenChanged -= value; }
            }
        }

        public void AssertNotNull()
        {
            Property = IsNull(Property) ? _property as IProperty<string> : Property;
            Assert.IsNotNull(Property);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_property == null && Property is MonoBehaviour mono && mono != null)
            {
                _property = mono;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_property != null)
            {
                Property = _property as IProperty<string>;
            }
        }

        private static bool IsNull(IProperty p) => p == null || (p is UnityEngine.Object obj && obj == null);
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(StringPropertyRef), true)]
    class StringPropertyRefDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _, GUIContent __) => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.PropertyField(rect, prop.FindPropertyRelative("_property"), label);
        }
    }
#endif
}
