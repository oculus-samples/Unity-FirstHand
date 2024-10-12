// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Compares the state of the player locomotor, counts the number of action that have been peformed
    /// </summary>
    public class LocomotorActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private PlayerLocomotor _playerLocomotor;
        [SerializeField]
        private ReferenceActiveState _shouldCountEvents;
        [SerializeField]
        private FloatRange _teleportEventCount = new FloatRange();
        [SerializeField]
        private FloatRange _rotationEventCount = new FloatRange();

        private List<LocomotionEvent> _events = new List<LocomotionEvent>();
        private bool _active;

        public bool Active => _active;

        private void OnEnable()
        {
            _playerLocomotor.WhenLocomotionEventHandled += UpdateActiveState;
            UpdateActiveState();
        }

        private void OnDisable()
        {
            _playerLocomotor.WhenLocomotionEventHandled -= UpdateActiveState;
        }

        private void UpdateActiveState(LocomotionEvent locomotionEvent, Pose delta)
        {
            if (!_shouldCountEvents) return;

            _events.Add(locomotionEvent);
            UpdateActiveState();
        }

        private void UpdateActiveState()
        {
            var active = true;

            var teleports = SumEvents(x => x.IsTeleport());
            active &= _teleportEventCount.Contains(teleports);

            var rotations = SumEvents(x => x.IsSnapTurn());
            active &= _rotationEventCount.Contains(rotations);

            _active = active;
        }

        int SumEvents(Predicate<LocomotionEvent> predicate)
        {
            int result = 0;
            _events.ForEach(x => result += (predicate(x) ? 1 : 0));
            return result;
        }
    }
}
