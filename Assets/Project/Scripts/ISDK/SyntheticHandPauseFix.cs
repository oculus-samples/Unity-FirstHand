// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Allows the synth hand to work while the game is paused
    /// </summary>
    public class SyntheticHandPauseFix : MonoBehaviour
    {
        [SerializeField]
        private SyntheticHand _hand;
        [SerializeField]
        private float _animationLength = 0.1f;

        private Func<float> _timeProvider = () => Time.unscaledTime;

        private void Reset()
        {
            _hand = GetComponent<SyntheticHand>();
        }

        private void Awake()
        {
            _hand.InjectJointLockCurve(Curve());
            _hand.InjectJointUnlockCurve(Curve());
            _hand.InjectWristPositionLockCurve(Curve());
            _hand.InjectWristPositionUnlockCurve(Curve());
            _hand.InjectWristRotationLockCurve(Curve());
            _hand.InjectWristRotationUnlockCurve(Curve());
        }

        private ProgressCurve Curve()
        {
            var result = new ProgressCurve();
            result.AnimationLength = _animationLength;
            result.SetTimeProvider(_timeProvider);
            return result;
        }
    }
}
