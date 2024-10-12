// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Places bubbles with the boundaries of the players room
    /// </summary>
    public class MRBubblePlacement : MonoBehaviour
    {
        [SerializeField]
        Transform _startPosition;
        [SerializeField]
        Transform _target;
        [SerializeField]
        private float _smoothTime = 4;
        [SerializeField]
        FollowTransform.Noise3 _noise;

        [SerializeField]
        ReferenceActiveState _active;

        private static Vector3[] _boundary;
        private OVRCameraRig _rig;
        private Vector3 _velocity;

        private void Start()
        {
            _noise.offset = Random.value * 10f;
            _rig = FindObjectOfType<OVRCameraRig>();
        }

        private void Update()
        {
            Vector3 targetPoint = GetTargetPoint();

            transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref _velocity, _smoothTime * Time.deltaTime, 0.4f);
            transform.rotation = Quaternion.LookRotation(transform.position - _target.position);
        }

        private Vector3 GetTargetPoint()
        {
            if (!_active)
            {
                return Vector3.MoveTowards(_startPosition.position, _target.position, 0.03f);
            }

            var point = _rig.trackingSpace.InverseTransformPoint(_target.position).SetY(0);
            var closestPoint = Vector3.ClampMagnitude(GetClosestPointOnBoundary(point), 0.9f);

            var targetPoint = _rig.trackingSpace.TransformPoint(closestPoint);
            targetPoint.y = _target.position.y;

            var playAreaCenter = new Vector3(_rig.trackingSpace.position.x, _rig.centerEyeAnchor.position.y - 0.3f, _rig.trackingSpace.position.z);

            targetPoint = Vector3.Lerp(playAreaCenter, targetPoint, 0.5f);

            targetPoint += _noise.Evaluate(Time.time);

            return targetPoint;
        }

        private static Vector3 GetClosestPointOnBoundary(Vector3 point)
        {
            _boundary ??= OVRManager.boundary?.GetGeometry(OVRBoundary.BoundaryType.PlayArea);

            if (_boundary == null || _boundary.Length == 0)
            {
                return Vector3.ClampMagnitude(point, 1f);
            }

            Vector3 bestPoint = Vector3.zero;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < _boundary.Length; i++)
            {
                var current = _boundary[i];
                var next = _boundary[(i + 1) % _boundary.Length];

                LineSegment line = new LineSegment(current, next);
                var pointOnLine = line.GetClosestPointOnLine(point);
                var distanceToPointOnLine = (point - pointOnLine).sqrMagnitude;

                if (distanceToPointOnLine < bestDistance)
                {
                    bestPoint = pointOnLine;
                    bestDistance = distanceToPointOnLine;
                }
            }

            return bestPoint;
        }
    }
}
