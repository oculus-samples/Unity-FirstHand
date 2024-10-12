// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Performs a raycast from _rayOrigin in Update and positions _target where the ray hits with an optional offset
    /// </summary>
    public class RaycastedPoser : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform _rayOrigin;
        [SerializeField]
        private float _positionOffset = 0.02f;
        [SerializeField]
        private OffsetMode _offsetMode;

        private bool _lastRayHit;
        public bool Active => _lastRayHit;

        private void Update()
        {
            transform.SetPose(_rayOrigin.GetPose());

            var ray = new Ray(transform.position, transform.forward);
            _lastRayHit = Physics.Raycast(ray, out var hit);

            if (_lastRayHit)
            {
                Vector3 offsetDirection = _offsetMode == OffsetMode.AlongNormal ? hit.normal : -ray.direction;
                Pose pose = new Pose(hit.point + offsetDirection * _positionOffset, Quaternion.LookRotation(hit.normal, transform.up));
                _target.SetPose(pose);
            }
        }

        private void LateUpdate()
        {
            transform.SetPose(_rayOrigin.GetPose());
        }

        enum OffsetMode
        {
            AlongRay,
            AlongNormal,
        }
    }
}
