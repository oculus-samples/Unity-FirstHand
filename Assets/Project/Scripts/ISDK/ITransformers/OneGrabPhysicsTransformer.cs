// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class OneGrabPhysicsTransformer : MonoBehaviour, ITransformer
    {
        private IGrabbable _grabbable;
        private Pose _grabDeltaInLocalSpace;

        private Pose _targetPose;
        private bool _grabbing;
        private Rigidbody _rigidbody;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
            _rigidbody = (_grabbable as Component).GetComponent<Rigidbody>();
        }

        public void BeginTransform()
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;
            _grabDeltaInLocalSpace = new Pose(targetTransform.InverseTransformVector(grabPoint.position - targetTransform.position),
                                            Quaternion.Inverse(grabPoint.rotation) * targetTransform.rotation);
            _grabbing = true;
            _targetPose = targetTransform.GetPose();
        }

        public void UpdateTransform()
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;
            _targetPose = new Pose(
                grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position),
                grabPoint.rotation * _grabDeltaInLocalSpace.rotation
                );
        }

        public void EndTransform() { _grabbing = false; }

        void FixedUpdate()
        {
            if (!_grabbing) return;
            SetRigidbodyVelocitiesForTarget(_targetPose.position, _targetPose.rotation);
        }


        void SetRigidbodyVelocitiesForTarget(Vector3 targetPosition, Quaternion targetRotation)
        {
            Rigidbody rb = _rigidbody;
            float deltaTime = Time.deltaTime;

            float velocityMagic = 6000f / (deltaTime / 0.0111f);
            float angularVelocityMagic = 50f / (deltaTime / 0.0111f);

            Quaternion rotationDelta;
            Vector3 positionDelta;

            float angle;
            Vector3 axis;

            rotationDelta = targetRotation * Quaternion.Inverse(rb.transform.rotation);
            positionDelta = (targetPosition - rb.transform.position);

            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
                angle -= 360;

            if (angle != 0)
            {
                Vector3 angularTarget = angle * axis;
                if (float.IsNaN(angularTarget.x) == false && deltaTime > 0)
                {
                    angularTarget = (angularTarget * angularVelocityMagic) * deltaTime;
                    rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, 200f);
                }
            }

            Vector3 velocityTarget = (positionDelta * velocityMagic) * deltaTime;
            if (float.IsNaN(velocityTarget.x) == false)
            {
                rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, 100f);
            }
        }
    }
}
