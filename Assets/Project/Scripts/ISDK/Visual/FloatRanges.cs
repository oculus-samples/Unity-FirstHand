// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Specifies a set of ranges using a string like "1-5, 8, 11-13"
    /// </summary>
    // TODO serialize the ranges instead of the string, only use the regex in edit mode
    [System.Serializable]
    public struct FloatRanges
    {
        /// <summary>
        /// Regex to parse "1-5, 8, 11-13, >=15" into 3 groups; comparison, value1 and value2
        /// </summary>
        private static Regex _rangeRegex = new Regex("([<>]=?)?(-?\\d+(?:\\.\\d+)?)\\s*(?:-\\s*(-?\\d+(?:\\.\\d+)?))?");

        [SerializeField]
        private string _range;

        private List<FloatRange> _ranges;

        public FloatRanges(int min, int max) : this()
        {
            _range = max != min ? $"{min}-{max}" : $"{min}";
        }

        public bool Contains(float value)
        {
            _ranges ??= ToRanges();
            return _ranges.FindIndex(x => x.Contains(value)) != -1 || _ranges.Count == 0; //empty means infinite range
        }

        public bool HasValue()
        {
            return !string.IsNullOrEmpty(_range);
        }

        List<FloatRange> ToRanges()
        {
            List<FloatRange> results = new List<FloatRange>();
            var match = _rangeRegex.Matches(_range);
            for (int i = 0; i < match.Count; i++)
            {
                var comparison = match[i].Groups[1].Value;
                var start = match[i].Groups[2].Value;
                var end = match[i].Groups[3].Value;
                results.Add(CreateRange(comparison, start, end));
            }
            return results;
        }

        private static FloatRange CreateRange(string comparison, string v1, string v2)
        {
            // if empty return an infinite range
            if (string.IsNullOrEmpty(v1)) { return new FloatRange(); }

            var value = float.Parse(v1);

            switch (comparison)
            {
                case ">": return new FloatRange() { Min = value, MinExclusive = true };
                case ">=": return new FloatRange() { Min = value };
                case "<": return new FloatRange() { Max = value, MaxExclusive = true };
                case "<=": return new FloatRange() { Max = value };
            }

            var value2 = !string.IsNullOrEmpty(v2) ? float.Parse(v2) : value;
            return new FloatRange(value, value2);
        }

        public static implicit operator FloatRanges(string d) => new FloatRanges() { _range = d };

        public override string ToString() => _ranges != null ? string.Join(", ", _ranges) : "";

        public float GetMin()
        {
            _ranges ??= ToRanges();
            float min = _ranges[0].Min;
            for (int i = 1; i < _ranges.Count; i++)
            {
                min = Mathf.Min(min, _ranges[i].Min);
            }
            return min;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FloatRanges))]
    class TextRangeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _, GUIContent __) => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.tooltip = string.IsNullOrEmpty(label.tooltip) ? "e.g. 1 - 5, 8, 11 - 13, >=15" : label.tooltip;
            var prop = property.FindPropertyRelative("_range");
            EditorGUI.PropertyField(position, prop, label);
            if (!prop.hasMultipleDifferentValues)
            {
                prop.stringValue = Regex.Replace(prop.stringValue, "[^-0-9.,<>= ]+", "");
            }
        }
    }
#endif
}
