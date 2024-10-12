// Copyright (c) Meta Platforms, Inc. and affiliates.

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls the colour based on UIState
    /// </summary>
    [ExecuteAlways]
    public class UIStateColorVisual : UIStateVisual
    {
        public UIColorSet _normal = new UIColorSet(Color.white, new Color(0.9f, 0.9f, 0.9f, 1f), new Color(0.8f, 0.8f, 0.8f, 1f), new Color(1, 1, 1, 0.5f));
        public UIColorSet _active = new UIColorSet(Color.white, new Color(0.9f, 0.9f, 0.9f, 1f), new Color(0.8f, 0.8f, 0.8f, 1f), new Color(1, 1, 1, 0.5f));

        [ControlsPrevious]
        public bool _useActive = true;
        public float _duration = 0.2f;

        private CanvasRenderer _renderer;

        protected override void OnEnable()
        {
            _renderer = GetComponent<CanvasRenderer>();
            base.OnEnable();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!_useActive) { _active.Mode = UIColorSet.Type.White; }
        }

        protected override void UpdateVisual(IUIState uiState, bool allowAnimation)
        {
#if UNITY_EDITOR
        if (!TryGetComponent(out _renderer)) return;
#endif
            TweenRunner.Kill(this);

            var color = (_useActive && uiState.Active ? _active : _normal).GetColor(uiState.State);
            if (!allowAnimation)
            {
                _renderer.SetColor(color);
            }
            else
            {
                var current = _renderer.GetColor();
                TweenRunner.Tween(current, color, _duration, x => _renderer.SetColor(x))
                    .IgnoreTimeScale()
                    .SetID(this);
            }
        }
    }

    [System.Serializable]
    public struct UIColorSet
    {
        [SerializeField] Type _type;
        [SerializeField] Color _normal;
        [SerializeField] Color _hovered;
        [SerializeField] Color _pressed;
        [SerializeField] Color _disabled;

        public UIColorSet(Color normal) : this()
        {
            _type = Type.Single;
            _normal = normal;
        }

        public UIColorSet(Color normal, Color hovered, Color pressed, Color disabled) : this(normal)
        {
            _type = Type.Full;
            _hovered = hovered;
            _pressed = pressed;
            _disabled = disabled;
        }

        public Type Mode { get => _type; set => _type = value; }

        public Color GetColor(UIStates state)
        {
            if (state == UIStates.None) { return Color.white; }

            if (_type == Type.Single)
            {
                return _normal;
            }
            else if (_type == Type.White)
            {
                switch (state)
                {
                    case UIStates.Disabled: return new Color(1, 1, 1, 0.5f);
                    default: return Color.white;
                }
            }
            else if (_type == Type.Clear)
            {
                return new Color(1, 1, 1, 0);
            }
            else if (_type == Type.ShowOnHover)
            {
                switch (state)
                {
                    case UIStates.Hovered:
                    case UIStates.Pressed: return Color.white;
                    default: return new Color(1, 1, 1, 0);
                }
            }
            else
            {
                switch (state)
                {
                    case UIStates.Normal: return _normal;
                    case UIStates.Hovered: return _hovered;
                    case UIStates.Pressed: return _pressed;
                    case UIStates.Disabled: return _disabled;
                    default: throw new System.Exception();
                }
            }
        }

        public enum Type
        {
            Single,
            Full,
            White,
            Clear,
            ShowOnHover
        }
    }

    public class ControlsPreviousAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UIColorSet))]
public class ColorSetDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var typeProp = property.FindPropertyRelative("_type");
        var type = typeProp.enumValueIndex;
        var lines = type == 0 ? 2 : type == 1 ? 5 : 1;
        return EditorGUIUtility.singleLineHeight * lines;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var typeProp = property.FindPropertyRelative("_type");
        EditorGUI.PropertyField(TakeLine(ref position), typeProp, label);

        var type = typeProp.enumValueIndex;

        if (type < 2)
        {
            EditorGUI.indentLevel++;
            var normal = property.FindPropertyRelative("_normal");

            bool single = typeProp.enumValueIndex == 0;
            EditorGUI.PropertyField(TakeLine(ref position), normal, new GUIContent(single ? " " : "Normal"));
            if (!single)
            {
                var hovered = property.FindPropertyRelative("_hovered");
                var pressed = property.FindPropertyRelative("_pressed");
                var disabled = property.FindPropertyRelative("_disabled");
                EditorGUI.PropertyField(TakeLine(ref position), hovered, new GUIContent("Hovered"));
                EditorGUI.PropertyField(TakeLine(ref position), pressed, new GUIContent("Pressed"));
                EditorGUI.PropertyField(TakeLine(ref position), disabled, new GUIContent("Disabled"));
            }
            EditorGUI.indentLevel--;
        }
    }

    public static Rect TakeLine(ref Rect rect, float spacing = 0)
    {
        var topHalf = rect;
        var bottomHalf = rect;
        topHalf.height = EditorGUIUtility.singleLineHeight;
        bottomHalf.height = rect.height - topHalf.height - spacing;
        bottomHalf.y += topHalf.height + spacing;
        rect = bottomHalf;
        return topHalf;
    }
}


[CustomPropertyDrawer(typeof(ControlsPreviousAttribute))]
public class ControlsPreviousDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.y -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        position.x -= 18;
        position.width = 18;
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property);
    }
}

#endif
}
