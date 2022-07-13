/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Specifies a Range for a Vector3
    /// </summary>
    [Serializable]
    public class Vector3Range : ValueRange<Vector3>
    {
        [field: SerializeField]
        public FloatRange X { get; private set; } = new FloatRange();
        [field: SerializeField]
        public FloatRange Y { get; private set; } = new FloatRange();
        [field: SerializeField]
        public FloatRange Z { get; private set; } = new FloatRange();

        public Vector3Range() { }

        public Vector3Range(FloatRange x, FloatRange y, FloatRange z)
        {
            X = x ?? throw new ArgumentNullException(nameof(x));
            Y = y ?? throw new ArgumentNullException(nameof(y));
            Z = z ?? throw new ArgumentNullException(nameof(z));
        }

        public override Vector3 Clamp(Vector3 vector)
        {
            return new Vector3(X.Clamp(vector.x), Y.Clamp(vector.y), Z.Clamp(vector.z));
        }

        public override Vector3 Random()
        {
            return new Vector3(X.Random(), Y.Random(), Z.Random());
        }

        public Vector3 Min => new Vector3(X.Min, Y.Min, Z.Min);
        public Vector3 Max => new Vector3(X.Max, Y.Max, Z.Max);

        public bool IsInfinity()
        {
            return float.IsInfinity(X.Size) && float.IsInfinity(Y.Size) && float.IsInfinity(Z.Size);
        }
    }

    [Serializable]
    public class IntRange : ValueRange<int>
    {
        [FormerlySerializedAs("x")]
        public int Min = int.MinValue;
        [FormerlySerializedAs("y")]
        public int Max = int.MaxValue;

        public override int Clamp(int value)
        {
            return Mathf.Clamp(value, Min, Max);
        }

        public override int Random()
        {
            return UnityEngine.Random.Range(Min, Max + 1);
        }
    }

    /// <summary>
    /// Specifies a range for a float
    /// </summary>
    [Serializable]
    public class FloatRange : ValueRange<float>
    {
        [FormerlySerializedAs("x")]
        public float Min = float.NegativeInfinity;
        [FormerlySerializedAs("y")]
        public float Max = float.PositiveInfinity;

        public FloatRange() { }

        public FloatRange(float min, float max)
        {
            bool ordered = max >= min;
            Min = ordered ? min : max;
            Max = ordered ? max : min;
        }

        public override float Clamp(float x)
        {
            return Mathf.Clamp(x, Min, Max);
        }

        public override string ToString()
        {
            if (Max == Min){ return Max.ToString(); }
            if (float.IsPositiveInfinity(Max) && !float.IsNegativeInfinity(Min)) { return $">{Min}"; }
            if (!float.IsPositiveInfinity(Max) && float.IsNegativeInfinity(Min)) { return $"<{Min}"; }
            if (float.IsPositiveInfinity(Max) && float.IsNegativeInfinity(Min)) { return "Infinity"; }
            return $"{Min}-{Max}";
        }

        public override float Random()
        {
            return UnityEngine.Random.Range(Min, Max);
        }

        public float Lerp(float t)
        {
            return Mathf.Lerp(Min, Max, t);
        }

        public float Size => Mathf.Abs(Max - Min);
    }

    public abstract class ValueRange<T> where T : struct
    {
        public abstract T Clamp(T value);

        public abstract T Random();

        public virtual bool Contains(T value)
        {
            return Clamp(value).Equals(value);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draws a FloatRange on a single line
    /// </summary>
    [CustomPropertyDrawer(typeof(FloatRange))]
    class RangePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _, GUIContent __) => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var r1 = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            EditorGUI.LabelField(r1, label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUIUtility.labelWidth = 30; // right size to fit "Min" and "Max"
            int spacing = 10;

            var r2 = new Rect(rect.x + r1.width, rect.y, (rect.width - r1.width) / 2 - spacing, rect.height);
            EditorGUI.PropertyField(r2, property.FindPropertyRelative("Min"));

            var r3 = new Rect(r2.x + r2.width + spacing, rect.y, r2.width, rect.height);
            EditorGUI.PropertyField(r3, property.FindPropertyRelative("Max"));

            EditorGUI.indentLevel = indent;
        }
    }
#endif
}
