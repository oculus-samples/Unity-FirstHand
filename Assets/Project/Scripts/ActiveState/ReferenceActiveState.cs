// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles IActiveState Interface boilerplate, allows inverting directly on the
    /// reference and has a nice property drawer.
    ///
    /// Example Usage:
    ///   [SerializeField]
    ///   private ReferenceActiveState _activeState;
    ///   protected bool Active => _activeState.Active;
    ///   protected virtual void Reset() => _activeState.InjectActiveState(GetComponent<IActiveState>());
    ///   protected virtual void Start() =>  _activeState.AssertNotNull();
    /// </summary>
    [System.Serializable]
    public struct ReferenceActiveState : ISerializationCallbackReceiver, IActiveState
    {
        [SerializeField, Interface(typeof(IActiveState))]
        private MonoBehaviour _activeState;
        [SerializeField]
        bool _invert;
        [SerializeField]
        bool _useNull;

        private string _profilerTag;

        public static ReferenceActiveState Optional()
        {
            return new ReferenceActiveState() { _useNull = true };
        }

        public IActiveState ActiveState { get; private set; }
        public bool Active => GetActive() != _invert;

        private bool GetActive()
        {
            //if (_profilerTag == null) _profilerTag = ActiveState.GetType().Name;
            Profiler.BeginSample("ReferenceActiveState.GetActive");
            bool active = ActiveState.Active;
            Profiler.EndSample();
            return active;
        }

        public bool HasReference => _activeState != null;

        public void InjectActiveState(IActiveState activeState)
        {
            ActiveState = activeState;
            _activeState = activeState as MonoBehaviour;
        }

        public void AssertNotNull(string message)
        {
            Assert.IsNotNull(ActiveState, message);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (ActiveState == null || (ActiveState as MonoBehaviour) == null)
            {
                if (_activeState && _activeState is IActiveState state)
                {
                    ActiveState = state;
                }
                else if (_useNull)
                {
                    ActiveState = new NullActiveState();
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        public static implicit operator bool(ReferenceActiveState activeState) => activeState.Active;

        public bool ReferenceEquals(ReferenceActiveState other)
        {
            return other.ActiveState == ActiveState && other._invert == _invert;
        }

        internal static ReferenceActiveState OptionalFalse()
        {
            return new ReferenceActiveState() { _useNull = true, _invert = true };
        }

        public struct NullActiveState : IActiveState
        {
            public bool Active => true;
        }
    }

    [System.Serializable]
    public struct ReferenceSelector : ISerializationCallbackReceiver, ISelector
    {
        [SerializeField, Interface(typeof(ISelector))]
        private MonoBehaviour _selector;
        public ISelector Selector { get; private set; }

        public event System.Action WhenSelected
        {
            add => Selector.WhenSelected += value;
            remove => Selector.WhenSelected -= value;
        }

        public event System.Action WhenUnselected
        {
            add => Selector.WhenUnselected += value;
            remove => Selector.WhenUnselected -= value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (Selector == null || (Selector as MonoBehaviour) == null)
            {
                if (_selector && _selector is ISelector state)
                {
                    Selector = state;
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }


#if UNITY_EDITOR
    /// <summary>
    /// Draws ReferenceActiveState on a single line
    /// </summary>
    [CustomPropertyDrawer(typeof(ReferenceActiveState))]
    public class ReferenceActiveStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var activeState = property.FindPropertyRelative("_activeState");
            var invert = property.FindPropertyRelative("_invert");
            var optionalProp = property.FindPropertyRelative("_useNull");

            var usedLabel = new GUIContent(label);
            if (optionalProp.boolValue)
            {
                usedLabel.text += " <color=#ffffff55>(Optional)</color>";
            }

            EditorGUI.BeginProperty(position, usedLabel, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorStyles.label.richText = true;

            // Calculate rects
            var activeStateRect = new Rect(position.x, position.y, position.width - 60, position.height);
            var invertToggleRect = new Rect(position.x + (position.width - 60) + 5, position.y, 60, position.height);

            EditorGUI.PropertyField(activeStateRect, activeState, usedLabel);
            EditorGUIUtility.labelWidth = 35;
            EditorGUI.PropertyField(invertToggleRect, invert, new GUIContent("Invert"));

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorStyles.label.richText = false;

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ReferenceSelector))]
    public class ReferenceSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var selector = property.FindPropertyRelative("_selector");
            EditorGUI.PropertyField(position, selector, label);
        }
    }

    [InitializeOnLoad]
    public class ReferenceActiveStateContextMenu
    {
        static ReferenceActiveStateContextMenu()
        {
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) return;
            if (property.name != "_activeState") return;

            var parent = FindParentProperty(property);
            if (parent == null) return;

            var optionalProp = parent.FindPropertyRelative("_useNull");
            if (optionalProp == null) return;

            var propertyCopy = optionalProp.Copy();
            menu.AddItem(new GUIContent("Optional"), optionalProp.boolValue, () =>
            {
                propertyCopy.boolValue = !propertyCopy.boolValue;
                property.serializedObject.ApplyModifiedProperties();
            });
        }

        public static SerializedProperty FindParentProperty(SerializedProperty serializedProperty)
        {
            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
            {
                return default;
            }

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths[0]);
            for (var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array")
                {
                    if (index + 1 == propertyPaths.Length - 1)
                    {
                        // reached the end
                        break;
                    }
                    if (propertyPaths.Length > index + 1 && System.Text.RegularExpressions.Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                        var arrayIndex = int.Parse(match.Groups[1].Value);
                        parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                        index++;
                    }
                }
                else
                {
                    parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
                }
            }

            return parentSerializedProperty;
        }
    }
#endif
}
