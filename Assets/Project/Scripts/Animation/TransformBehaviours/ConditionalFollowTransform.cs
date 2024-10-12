// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Enables/Disables a FollowTransform based on an ActiveState using a tween
    /// Useful for when you have a character whose head is looking at the player using a
    /// FollowTransform, but you only want them to look when the player is in some zone
    /// </summary>
    [RequireComponent(typeof(FollowTransform))]
    public class ConditionalFollowTransform : ActiveStateObserver
    {
        [SerializeField]
        private bool _position = true;
        [SerializeField]
        private bool _rotation = true;
        [SerializeField]
        private float _duration;
        [SerializeField]
        private Tween.Ease _ease = Tween.Ease.Linear;

        private FollowTransform _followTransform;
        private float _weight = 0;

        protected override void Start()
        {
            base.Start();

            _followTransform = GetComponent<FollowTransform>();
            SetWeight(Active ? 1 : 0);
        }

        protected override void HandleActiveStateChanged()
        {
            TweenRunner.Kill(this);
            int weight = Active ? 1 : 0;
            if (_duration > 0)
            {
                TweenRunner.Tween(_weight, weight, _duration, SetWeight).SetEase(_ease).SetID(this);
            }
            else
            {
                SetWeight(weight);
            }
        }

        private void SetWeight(float weight)
        {
            _weight = weight;
            if (_position) _followTransform.PositionWeight = weight;
            if (_rotation) _followTransform.RotationWeight = weight;
        }
    }
}
