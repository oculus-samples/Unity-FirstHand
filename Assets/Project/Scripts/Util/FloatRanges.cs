/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

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

            switch (comparison) //TODO exclusive
            {
                case ">":
                case ">=": return new FloatRange() { Min = value };
                case "<":
                case "<=": return new FloatRange() { Max = value };
            }

            var value2 = !string.IsNullOrEmpty(v2) ? float.Parse(v2) : value;
            return new FloatRange(value, value2);
        }

        public static implicit operator FloatRanges(string d) => new FloatRanges() { _range = d };

        public override string ToString() => _ranges != null ? string.Join(", ", _ranges) : "";
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FloatRanges))]
    class TextRangeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _, GUIContent __) => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.tooltip = string.IsNullOrEmpty(label.tooltip) ? "e.g. 1 - 5, 8, 11 - 13" : label.tooltip;
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
