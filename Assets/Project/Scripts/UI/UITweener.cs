// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// General UI tweener
    /// </summary>
    public class UITweener : UIStateVisual
    {
        [SerializeField]
        private float _duration;
        [SerializeField]
        private Quaternion _normalRotation;
        [SerializeField]
        private Quaternion _hoverRotation;
        [SerializeField]
        private Quaternion _pressRotation;
        [SerializeField]
        private Tween.Ease _ease;

        protected override void UpdateVisual(IUIState uiState, bool animate)
        {
#if UNITY_EDITOR
            if (transform == null) { return; }
#endif
            Quaternion rotation = GetRotationForState(uiState.State);
            TweenRunner.TweenRotationLocal(transform, rotation, _duration)
                .IgnoreTimeScale()
                .SetID(this)
                .SetEase(_ease)
                .Skip(!animate);
        }

        private Quaternion GetRotationForState(UIStates state)
        {
            switch (state)
            {
                case UIStates.None:
                case UIStates.Normal:
                case UIStates.Disabled: return _normalRotation;
                case UIStates.Hovered: return _hoverRotation;
                case UIStates.Pressed: return _pressRotation;
                default: throw new Exception();
            }
        }
    }
}
