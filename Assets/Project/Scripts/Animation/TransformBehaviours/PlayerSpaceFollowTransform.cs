// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Makes a follow transform act as if it were in the players local space by removing smoothing
    /// </summary>
    public class PlayerSpaceFollowTransform : MonoBehaviour
    {
        [SerializeField]
        PlayerLocomotor _playerLocomotor;

        GloveJointTracker[] _gloveJointTrackers;
        FollowTransform[] _followTransforms;

        private void OnEnable()
        {
            _followTransforms = GetComponentsInChildren<FollowTransform>(true);
            _gloveJointTrackers = GetComponentsInChildren<GloveJointTracker>(true);
            _playerLocomotor.WhenLocomotionEventHandled += UpdateFollowers;
        }

        private void OnDisable()
        {
            if (_playerLocomotor)
            {
                _playerLocomotor.WhenLocomotionEventHandled -= UpdateFollowers;
            }
        }

        private void UpdateFollowers(LocomotionEvent locomotionEvent, Pose _)
        {
            if (locomotionEvent.IsTeleport() || locomotionEvent.IsSnapTurn())
            {
                for (int i = 0; i < _gloveJointTrackers.Length; i++)
                {
                    _gloveJointTrackers[i].UpdatePose();
                }

                for (int i = 0; i < _followTransforms.Length; i++)
                {
                    _followTransforms[i].UpdatePose(smoothing: false);
                }
            }
        }
    }
}
