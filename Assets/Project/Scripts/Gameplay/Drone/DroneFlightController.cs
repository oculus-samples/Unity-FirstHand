// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls the drone's flight around the scene, using the "_flightAnimator" animator component to handle movement.
    /// Randomly chooses flight sequence animations and plays them on the animator, idling for a random time between
    /// sequences.
    /// </summary>
    public class DroneFlightController : MonoBehaviour
    {
        [SerializeField] private Animator _flightAnimator;
        [SerializeField] private Animator _droneAnimator;
        [SerializeField] private FloatRange _idleTimeRangeSeconds;
        [SerializeField] private int _flightSequenceCount;

        public bool IsInIdleState { get; private set; }

        private int _flyParameter = Animator.StringToHash("Fly");
        private int _flightSequenceParameter = Animator.StringToHash("FlightSequence");

        private List<DroneActionVolume> _currentActionVolumes = new List<DroneActionVolume>();
        public event Action WhenVolumesChanged;

        /// <summary>
        /// Used to gradually slowdown the drone when it stops moving instead of a hard stop
        /// </summary>
        private CustomAnimatorUpdate _animatorUpdate;

        public bool Moving { get; private set; } = true;
        public bool Flying { get; private set; }

        private void Awake()
        {
            Assert.IsTrue(_flightSequenceCount > 0);
            _animatorUpdate = _flightAnimator.GetComponent<CustomAnimatorUpdate>();
            HandleEnteredIdleFlightState();
        }

        /// <summary>
        /// After a flight sequence is complete, idle for a random period of time and then start a new sequence.
        /// </summary>
        public void HandleEnteredIdleFlightState()
        {
            StartCoroutine(IdleAndChooseNewFlightSequence());
            IEnumerator IdleAndChooseNewFlightSequence()
            {
                IsInIdleState = true;

                _flightAnimator.SetInteger(_flightSequenceParameter, 0);
                yield return new WaitForSeconds(_idleTimeRangeSeconds.Random());

                IsInIdleState = false;

                int flightSequence = Random.Range(0, _flightSequenceCount) + 1;
                _flightAnimator.SetInteger(_flightSequenceParameter, flightSequence);
            }
        }

        /// <summary>
        /// Keeps track of action volumes that the drone is currently within.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DroneActionVolume actionVolume))
            {
                _currentActionVolumes.Add(actionVolume);
                WhenVolumesChanged?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out DroneActionVolume actionVolume) &&
                _currentActionVolumes.Remove(actionVolume))
            {
                WhenVolumesChanged?.Invoke();
            }
        }

        public bool CurrentActionVolumesContains(Predicate<DroneActionVolume> predicate)
        {
            return _currentActionVolumes.FindIndex(predicate) != -1;
        }

        public void Move(bool move)
        {
            if (Moving == move) return;

            Moving = move;
            TweenRunner.Tween(_animatorUpdate.speedMultiplier, move ? 1 : 0, 0.5f, x => _animatorUpdate.speedMultiplier = x);
        }

        public void SetFlying(bool value)
        {
            if (Flying == value) return;

            Flying = value;
            _flightAnimator.SetBool(_flyParameter, value);
            _droneAnimator.SetBool(_flyParameter, value);
        }
    }
}
