// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction
{
    /// <summary>
    /// A Transformer that moves the target in a 1-1 fashion with the GrabPoint.
    /// Updates transform the target in such a way as to maintain the target's
    /// local positional and rotational offsets from the GrabPoint.
    /// </summary>
    public class OneGrabFreeTransformerClosestToHand : MonoBehaviour, ITransformer
    {
        private IGrabbable _grabbable;
        private Pose _grabDeltaInLocalSpace;
        private const float _maxDistance = 0.05f;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
        }

        public void BeginTransform()
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;

            _grabDeltaInLocalSpace = new Pose(targetTransform.InverseTransformVector(grabPoint.position - targetTransform.position),
                                            Quaternion.Inverse(grabPoint.rotation) * targetTransform.rotation);

            _grabDeltaInLocalSpace.position = Vector3.ClampMagnitude(_grabDeltaInLocalSpace.position, _maxDistance);
        }

        public void UpdateTransform()
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;
            targetTransform.rotation = grabPoint.rotation * _grabDeltaInLocalSpace.rotation;
            targetTransform.position = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);
        }

        public void EndTransform() { }
    }
}
