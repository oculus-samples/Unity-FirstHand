// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Grab.GrabSurfaces;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Constrains a FollowTransform to it's closest point a bezier path
    /// </summary>
    [RequireComponent(typeof(FollowTransform))]
    public class BezierFollowConstraint : MonoBehaviour
    {
        [SerializeField]
        private BezierGrabSurface _bezierSurface;
        [SerializeField]
        private float _weight = 1;

        public float Weight { get => _weight; set => _weight = value; }

        private void OnEnable()
        {
            GetComponent<FollowTransform>().WhenTransformUpdated += ConstrainToBezier;
        }

        private void OnDisable()
        {
            GetComponent<FollowTransform>().WhenTransformUpdated -= ConstrainToBezier;
        }

        private void ConstrainToBezier()
        {
            if (_weight <= 0) return;

            var pose = transform.GetPose();
            _bezierSurface.CalculateBestPoseAtSurface(pose, out var result, new Grab.PoseMeasureParameters(0), transform);
            transform.position = Vector3.Lerp(pose.position, result.position, _weight);
        }
    }
}
