// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using static Oculus.Interaction.ComprehensiveSample.ConditionalSetParent;
using static Oculus.Interaction.ComprehensiveSample.Tween;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Updates the active source based on the conditional transform active state value
    /// </summary>
    [ExecuteAlways]
    public class ConditionalFollowTransformSource : MonoBehaviour
    {
        [SerializeField]
        private List<ConditionalTransform> _sources = new List<ConditionalTransform>();
        [SerializeField]
        private Transform _default;
        [SerializeField]
        private float _duration;
        [SerializeField]
        private Ease _ease;
        [SerializeField]
        private bool _durationIsSpeed;

        private FollowTransform _followTransform;

        private void Awake()
        {
            _followTransform = GetComponent<FollowTransform>();
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                UpdateActiveParent(true);
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                EditorUpdate();
                return;
            }

            UpdateActiveParent();
        }

        private void UpdateActiveParent(bool instant = false)
        {
            var activeParent = GetActiveParent();
            if (IsAssigned(activeParent)) return;

            SetWeight(0);
            var previousSource = _followTransform.Source;
            _followTransform.Source = activeParent;
            var duration = _duration;
            if (_durationIsSpeed)
            {
                var dist = (_followTransform.transform.position - activeParent.position).magnitude;
                duration = dist / _duration;
            }

            if (_ease.IsClamped01())
            {
                TweenRunner.Tween(0, 1, duration, SetWeight)
                    .SetEase(_ease)
                    .SetID(this)
                    .Skip(instant);
            }
            else
            {
                SetWeight(1);
                var tPos = _followTransform.positionSettings.enabled;
                var tRot = _followTransform.rotationSettings.enabled;
                var current = transform.GetPose();
                TweenRunner.Tween01(duration, x =>
                {
                    var target = activeParent.GetPose();
                    if (previousSource) current = previousSource.GetPose();
                    if (tPos) transform.position = Vector3.LerpUnclamped(current.position, target.position, x);
                    if (tRot) transform.rotation = Quaternion.LerpUnclamped(current.rotation, target.rotation, x);
                })
                    .SetEase(_ease)
                    .SetUpdate(UpdateTime.LateUpdate)
                    .SetID(this)
                    .Skip(instant);
            }
        }

        private void SetWeight(float weight)
        {
            _followTransform.PositionWeight = _followTransform.RotationWeight = weight;
        }

        private Transform GetActiveParent()
        {
            var activeIndex = _sources.FindIndex(x => x.Active);
            return activeIndex >= 0 ? _sources[activeIndex].Transform : _default;
        }

        private bool IsAssigned(Transform activeParent)
        {
            if (_followTransform.Source == activeParent) return true;

            if (_followTransform._assignMainCamera && activeParent == null)
            {
                return _followTransform.Source.CompareTag("MainCamera");
            }

            return false;
        }

        private void EditorUpdate()
        {
            if (_followTransform == null || !_followTransform.UpdateInEditMode) return;

            var oldSource = _followTransform.Source;
            _followTransform.Source = GetActiveParent();
            _followTransform.UpdatePose(false);
            _followTransform.Source = oldSource;
        }
    }
}
