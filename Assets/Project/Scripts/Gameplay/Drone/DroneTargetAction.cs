// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Drone AI action that plays the target deployed animation for the duration of the action.
    /// </summary>
    public class DroneTargetAction : DroneAction
    {
        private int _deployTargetAnimationParameter;

        [SerializeField]
        GameObject _targetPrefab;
        [SerializeField]
        Transform _targetPose;

        List<GameObject> _targets = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();

            _deployTargetAnimationParameter = Animator.StringToHash("DeployTarget");
        }

        protected override bool CanPerformAction()
        {
            if (!FlightController.Flying && _targets.Count > 0)
            {
                CleanUp();
            }

            // this action can only be performed when the drone is in an action volume with the
            // "CanDeployTargetInVolume" flag set to true.
            return FlightController.Flying && FlightController.Moving && CurrentActionVolumesContains(x => x.CanDeployTargetInVolume);
        }

        protected override void StartAction()
        {
            ActionAnimator.SetBool(_deployTargetAnimationParameter, true);
            TweenRunner.DelayedCall(1f, () =>
            {
                var target = Instantiate(_targetPrefab);
                target.transform.SetPositionAndRotation(_targetPose.position, _targetPose.rotation);
                _targets.Add(target);
            });
            FlightController.Move(false);
        }

        protected override void EndAction()
        {
            ActionAnimator.SetBool(_deployTargetAnimationParameter, false);
            FlightController.Move(true);
        }

        protected override void HandleVolumeChanged() { }

        public void CleanUp()
        {
            _targets.ForEach(x => { if (x) Destroy(x); });
            _targets.Clear();
        }
    }
}
