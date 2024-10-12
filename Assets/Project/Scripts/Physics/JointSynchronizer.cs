// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Detects if a joints connected body has teleported and resets the joint when this occurs
    /// </summary>
    public class JointSynchronizer : MonoBehaviour
    {
        [SerializeField, Tooltip("The distance the connected body must move in a single frame to be considered a teleport")]
        private float _maxDistance = float.PositiveInfinity;

        [SerializeField, Tooltip("The angle the connected body must move in a single frame to be considered a teleport")]
        private float _maxAngle = float.PositiveInfinity;

        private Pose _lastPose;
        private Joint _joint;

        private void Start()
        {
            _joint = GetComponent<Joint>();
            _lastPose = _joint.connectedBody.transform.GetPose();
        }

        void Update()
        {
            var pose = _joint.connectedBody.transform.GetPose();

            var distanceOffset = pose.position - _lastPose.position;
            var angleOffset = Vector3.Angle(pose.forward, _lastPose.forward);

            if (distanceOffset.sqrMagnitude > _maxDistance * _maxDistance || angleOffset > _maxAngle)
            {
                _joint.autoConfigureConnectedAnchor = false;
                _joint.transform.SetPose(pose);
                _joint.autoConfigureConnectedAnchor = true;
            }

            _lastPose = pose;
        }
    }
}
