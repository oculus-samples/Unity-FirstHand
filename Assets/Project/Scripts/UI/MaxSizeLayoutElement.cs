// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Limits the layout element to a max and min scale
    /// </summary>
    public class MaxSizeLayoutElement : UIBehaviour, ILayoutElement
    {
        private static List<ILayoutElement> _layoutElements = new List<ILayoutElement>();

        [SerializeField]
        private int _priority = 3;
        public Vector2 MaxSize = new Vector2(-1, -1);

        public float minWidth => GetLayoutSize(MaxSize.x, ValueType.MinWidth);
        public float preferredWidth => GetLayoutSize(MaxSize.x, ValueType.PreferredWidth);
        public float flexibleWidth => -1;

        public float minHeight => GetLayoutSize(MaxSize.y, ValueType.MinHeight);
        public float preferredHeight => GetLayoutSize(MaxSize.y, ValueType.PreferredHeight);
        public float flexibleHeight => -1;

        public int layoutPriority => _priority;

        private float GetLayoutSize(float value, ValueType type)
        {
            if (value < 0) { return -1; }

            var otherValue = GetSizeForValueType(type);
            if (otherValue < 0) { return -1; }

            return Mathf.Min(otherValue, value);
        }

        float GetValue(ILayoutElement element, ValueType type)
        {
            switch (type)
            {
                case ValueType.MinWidth: return element.minWidth;
                case ValueType.MinHeight: return element.minHeight;
                case ValueType.PreferredWidth: return element.preferredWidth;
                case ValueType.PreferredHeight: return element.preferredHeight;
                default: throw new Exception();
            }
        }

        private float GetSizeForValueType(ValueType type)
        {
            float bestSize = -1;
            float bestPriority = int.MinValue;

            GetComponents(_layoutElements);

            for (int i = 0; i < _layoutElements.Count; i++)
            {
                if (_layoutElements[i] == (ILayoutElement)this) { continue; }
                if (_layoutElements[i] is MonoBehaviour behaviour && !behaviour.isActiveAndEnabled) { continue; }

                int priority = _layoutElements[i].layoutPriority;
                if (priority >= layoutPriority || priority < bestPriority) { continue; }

                var size = GetValue(_layoutElements[i], type);
                if (size > bestSize)
                {
                    bestPriority = priority;
                    bestSize = size;
                }
            }

            return bestSize;
        }

        private void MarkForRebuild()
        {
            if (isActiveAndEnabled)
            {
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate() => MarkForRebuild();
#endif

        void ILayoutElement.CalculateLayoutInputHorizontal() { }
        void ILayoutElement.CalculateLayoutInputVertical() { }

        enum ValueType
        {
            MinWidth, MinHeight,
            PreferredWidth, PreferredHeight
        }
    }
}
