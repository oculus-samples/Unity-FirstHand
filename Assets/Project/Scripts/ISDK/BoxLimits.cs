// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// An example of a class that can impose optional negative and positive limits on a Vector3 and uses an inspector that's 3 lines
    /// </summary>
    public class BoxLimits : MonoBehaviour
    {
        [SerializeField]
        Vector3Range _range;
    }

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

        public static Vector3Range Infinity => new Vector3Range();
        public static Vector3Range Zero => new Vector3Range(FloatRange.Zero, FloatRange.Zero, FloatRange.Zero);

        public bool IsInfinity()
        {
            return float.IsInfinity(X.Size) && float.IsInfinity(Y.Size) && float.IsInfinity(Z.Size);
        }

        public override bool Contains(Vector3 value)
        {
            return Clamp(value) == value;
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

        public override bool Contains(int value)
        {
            return Clamp(value) == value;
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

        public bool MinExclusive = false;
        public bool MaxExclusive = false;

        public FloatRange() { }

        public FloatRange(float min, float max)
        {
            bool ordered = max >= min;
            Min = ordered ? min : max;
            Max = ordered ? max : min;
        }

        public FloatRange(float min, bool minExclusive, float max, bool maxExclusive) : this(min, max)
        {
            MinExclusive = minExclusive;
            MaxExclusive = maxExclusive;
        }

        public override float Clamp(float x)
        {
            var different = Max != Min;
            var min = different && MinExclusive ? FloatDelta.Increment(Min) : Min;
            var max = different && MaxExclusive ? FloatDelta.Decrement(Max) : Max;
            return Mathf.Clamp(x, min, max);
        }

        public override string ToString()
        {
            if (Max == Min) { return Max.ToString(); }
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

        public float InverseLerp(float t)
        {
            return Mathf.InverseLerp(Min, Max, t);
        }

        public float Size => Mathf.Abs(Max - Min);

        public bool IsPositiveInfinity => float.IsPositiveInfinity(Min) && float.IsPositiveInfinity(Max);
        public bool IsInfinity => float.IsNegativeInfinity(Min) && float.IsPositiveInfinity(Max);

        public override bool Contains(float value)
        {
            if (IsInfinity) return true;
            return Clamp(value) == value;
        }

        public static FloatRange Infinity => new FloatRange();
        public static FloatRange Zero => new FloatRange(0, 0);

        //https://stackoverflow.com/a/59273138
        [StructLayout(LayoutKind.Explicit)]
        struct FloatDelta
        {
            [FieldOffset(0)] private float _f;
            [FieldOffset(0)] private int _i;
            public static int Convert(float value) => new FloatDelta { _f = value }._i;
            public static float Convert(int value) => new FloatDelta { _i = value }._f;

            //https://stackoverflow.com/a/14278361
            public static float Increment(float f)
            {
                int val = Convert(f);
                if (f > 0) val++;
                else if (f < 0) val--;
                else if (f == 0) return float.Epsilon;
                return Convert(val);
            }

            public static float Decrement(float f)
            {
                int val = Convert(f);
                if (f > 0) val--;
                else if (f < 0) val++;
                else if (f == 0) return -float.Epsilon;
                return Convert(val);
            }
        }

    }

    public abstract class ValueRange<T> where T : struct
    {
        public abstract T Clamp(T value);

        public abstract T Random();

        public abstract bool Contains(T value);
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
