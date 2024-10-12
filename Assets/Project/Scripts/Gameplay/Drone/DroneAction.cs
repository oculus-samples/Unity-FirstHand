// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Base class for drone AI actions. Actions are coupled to the "ActionAnimator" Animator component,
    /// and will begin only if this animator is in the idle state (ie no other actions are active).
    /// </summary>
    public abstract class DroneAction : MonoBehaviour
    {
        [SerializeField] private Animator _actionAnimator;
        protected Animator ActionAnimator => _actionAnimator;

        [SerializeField] private FloatRange _waitTimeRangeSeconds = new FloatRange(0, 1);
        [SerializeField] private FloatRange _actionDurationRangeSeconds = new FloatRange(6, 10);
        private float _waitTimeRemaining;
        private float _actionTimeRemaining;

        [SerializeField]
        protected DroneFlightController FlightController;
        private DroneIdleState _idleState;
        protected bool IsInIdleState { get; private set; }
        public bool ActionIsActive { get; private set; }

        protected virtual void Awake()
        {
            Assert.IsNotNull(FlightController);

            _idleState = _actionAnimator.GetBehaviour<DroneIdleState>();
            Assert.IsNotNull(_idleState);
        }

        private void OnEnable()
        {
            _idleState.WhenEnteredIdleState += HandleEnteredIdleActionState;
            _idleState.WhenExitedIdleState += HandleExitedIdleActionState;
            FlightController.WhenVolumesChanged += HandleVolumeChanged;
        }

        private void OnDisable()
        {
            if (FlightController)
            {
                FlightController.WhenVolumesChanged -= HandleVolumeChanged;
            }

            if (_idleState)
            {
                _idleState.WhenEnteredIdleState -= HandleEnteredIdleActionState;
                _idleState.WhenExitedIdleState -= HandleExitedIdleActionState;
            }
        }

        protected virtual void Update()
        {
            if (ActionIsActive)
            {
                _actionTimeRemaining -= Time.deltaTime;
                if (_actionTimeRemaining < 0)// || !CanPerformAction())
                {
                    EndAction();
                    ActionIsActive = false;

                    _waitTimeRemaining = _waitTimeRangeSeconds.Random();
                }
            }
            else if (CanPerformAction())
            {
                _waitTimeRemaining -= Time.deltaTime;
                if (_waitTimeRemaining < 0 && !ActionIsActive)
                {
                    PerformAction();
                }
            }
        }

        private void HandleEnteredIdleActionState()
        {
            IsInIdleState = true;
        }

        private void HandleExitedIdleActionState()
        {
            IsInIdleState = false;
        }

        protected bool CurrentActionVolumesContains(Predicate<DroneActionVolume> predicate)
        {
            return FlightController.CurrentActionVolumesContains(predicate);
        }

        protected abstract bool CanPerformAction();
        protected abstract void StartAction();
        protected abstract void EndAction();
        protected abstract void HandleVolumeChanged();

        public void PerformAction()
        {
            ActionIsActive = true;
            StartAction();
            _actionTimeRemaining = _actionDurationRangeSeconds.Random();
        }
    }
}
