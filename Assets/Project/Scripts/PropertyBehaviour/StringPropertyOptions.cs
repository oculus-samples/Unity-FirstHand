// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Limits a StringProperty to a set of options and provides
    /// Previous/Next functionality to switch between them
    /// </summary>
    public class StringPropertyOptions : MonoBehaviour
    {
        [SerializeField]
        private StringPropertyRef _stringProperty;
        [SerializeField, StringPropOption]
        private List<string> _options = new List<string>();
        [SerializeField, Tooltip("When true, a call to Next() when on the last value " +
            "will loop to the value at index 0, and similar for a call to Previous()")]
        private bool _loop;

        public int CurrentIndex => _options.IndexOf(_stringProperty.Value);

        private void Reset()
        {
            _stringProperty.Property = GetComponent<IProperty<string>>();
        }

        private void Start()
        {
            _stringProperty.AssertNotNull();
            _stringProperty.WhenChanged += AssetValueIsOption;
        }

        private void OnDestroy()
        {
            _stringProperty.WhenChanged -= AssetValueIsOption;
        }

        private void AssetValueIsOption()
        {
            Assert.IsTrue(_options.Contains(_stringProperty.Value), $"{_stringProperty.Value} is not an option");
        }

        public void Next()
        {
            SetCurrent(ClampIndex(CurrentIndex + 1));
        }

        public void Previous()
        {
            SetCurrent(ClampIndex(CurrentIndex - 1));
        }

        private int ClampIndex(int index)
        {
            return _loop ? (int)Mathf.Repeat(index, _options.Count) : Mathf.Clamp(index, 0, _options.Count - 1);
        }

        private void SetCurrent(int index)
        {
            _stringProperty.Value = _options[index];
        }


        public class StringPropOptionAttribute : PropertyAttribute { }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(StringPropOptionAttribute))]
        public class StringPropOptionDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var right = position;
                var left = position;
                right.width = 30;
                left.width -= right.width;
                right.x += left.width;
                left.width -= 2;

                property.stringValue = EditorGUI.TextField(left, label, property.stringValue);

                bool canSet = Application.isPlaying && !property.serializedObject.isEditingMultipleObjects;
                EditorGUI.BeginDisabledGroup(!canSet);
                if (GUI.Button(right, "Set"))
                {
                    var stringProp = property.serializedObject
                        .FindProperty("_stringProperty")
                        .FindPropertyRelative("_property").objectReferenceValue
                        as IProperty<string>;

                    if (stringProp != null)
                    {
                        stringProp.Value = property.stringValue;
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
        }
#endif
    }
}
