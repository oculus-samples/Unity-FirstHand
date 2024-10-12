// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Becomes Active when a Transform's position/rotation/scale properties fall within the specified ranges
    /// </summary>
    public class TransformActiveState : MonoBehaviour, IActiveState
    {
        public Transform target;
        public Transform space;

        public Vector3Range PositionRange = Vector3Range.Infinity;
        public Vector3Range EulerAngleRange = Vector3Range.Infinity; //TODO better angle handling

        public Vector3Range Velocity = Vector3Range.Infinity;

        public FloatRange Speed = FloatRange.Infinity;
        public FloatRange Acceleration = FloatRange.Infinity;

        private Vector3 _velocity;
        private float _acceleration;

        private Vector3 _lastPosition;

        public bool Active
        {
            get
            {
                var pose = GetPose();
                return
                    PositionRange.Contains(pose.position) &&
                    EulerAngleRange.Contains(pose.rotation.eulerAngles) &&
                    Velocity.Contains(_velocity) &&
                    Acceleration.Contains(_acceleration) &&
                    Speed.Contains(_velocity.magnitude);
            }
        }

        private void Reset()
        {
            target = transform;
            space = transform.parent;
        }

        private void Awake()
        {
            target = target ? target : transform;
            space = space ? space : transform.parent;
        }

        private void Update()
        {
            if (!target) return;

            var position = space ? space.InverseTransformPoint(target.position) : target.position;
            var deltaPosition = position - _lastPosition;
            _lastPosition = position;

            if (PauseHandler.IsTimeStopped) return;

            var velocity = deltaPosition / Time.deltaTime;
            var deltaVelocity = velocity - _velocity;
            _velocity = velocity;
            _acceleration = Vector3.Dot(deltaVelocity, _velocity.normalized) / Time.deltaTime;
        }

        public override string ToString()
        {
            var pose = GetPose();
            var nl = Environment.NewLine;
            return $"Active: {Active}{nl}" +
                $"Position: {pose.position} {PositionRange.Contains(pose.position)}{nl}" +
                $"Euler: {pose.rotation.eulerAngles} {EulerAngleRange.Contains(pose.rotation.eulerAngles)}{nl}" +
                $"Velocity: {_velocity} {Velocity.Contains(_velocity)}{nl}" +
                $"Speed: {_velocity.magnitude} {Speed.Contains(_velocity.magnitude)}{nl}" +
                $"Accel: {_acceleration} {Acceleration.Contains(_acceleration)}{nl}";
        }

        Pose GetPose()
        {
            if (!target) { return Pose.identity; }

            var pose = target.GetPose();
            if (space)
            {
                return new Pose()
                {
                    position = space.InverseTransformPoint(pose.position),
                    rotation = Quaternion.Inverse(space.rotation) * pose.rotation
                };
            }
            else
            {
                return pose;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!target || EulerAngleRange.IsInfinity()) { return; }

            Vector3 min = RemoveInfinity(EulerAngleRange.Min);
            var minRot = (space ? space.rotation : Quaternion.identity) * Quaternion.Euler(min);

            Vector3 max = RemoveInfinity(EulerAngleRange.Max);
            var maxRot = (space ? space.rotation : Quaternion.identity) * Quaternion.Euler(max);

            Gizmos.DrawLine(target.position, target.position + minRot * Vector3.forward);
            Gizmos.DrawLine(target.position, target.position + maxRot * Vector3.forward);

            Vector3 RemoveInfinity(Vector3 v3)
            {
                for (int i = 0; i < 3; i++) if (float.IsInfinity(v3[i])) v3[i] = 0;
                return v3;
            }
        }
    }
}
