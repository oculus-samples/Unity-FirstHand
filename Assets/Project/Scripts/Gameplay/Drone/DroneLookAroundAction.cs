// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Drone AI action that plays the "look around" animation sequence.
    /// </summary>
    public class DroneLookAroundAction : DroneAction
    {
        private int _lookAroundAnimationParameter;

        protected override void Awake()
        {
            base.Awake();

            _lookAroundAnimationParameter = Animator.StringToHash("LookAround");
        }

        protected override bool CanPerformAction()
        {
            return FlightController.Flying && FlightController.IsInIdleState;
        }

        protected override void StartAction()
        {
            ActionAnimator.SetTrigger(_lookAroundAnimationParameter);
        }

        protected override void EndAction() { }

        protected override void HandleVolumeChanged() { }
    }
}
