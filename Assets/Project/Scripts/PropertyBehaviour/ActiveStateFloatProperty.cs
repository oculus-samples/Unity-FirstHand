// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Tweens a FloatProperty between 2 values, when the ActiveState changes
    /// used to fade out the ghost hands visibility when the users hand is very close to them
    /// </summary>
    public class ActiveStateFloatProperty : ActiveStateObserver
    {
        [SerializeField]
        FloatProperty _floatProperty;
        [SerializeField]
        float _changeDuration = 0.4f;
        [SerializeField]
        FloatRange _range;
        [SerializeField]
        Tween.Ease _ease = Tween.Ease.Linear;

        protected override void HandleActiveStateChanged()
        {
            float targetValue = Active ? _range.Max : _range.Min;
            float duration = Mathf.Abs(targetValue - _floatProperty.Value) / _range.Size * _changeDuration;
            TweenRunner.Tween(_floatProperty.Value, targetValue, duration, x => _floatProperty.Value = x).SetID(this).SetEase(_ease);
        }
    }
}
