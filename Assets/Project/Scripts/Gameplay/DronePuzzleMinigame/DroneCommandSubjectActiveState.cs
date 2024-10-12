// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DroneCommandSubjectActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private DroneCommandSubject _subject;
        [SerializeField]
        private ActiveStateExpectation _hasCommands = ActiveStateExpectation.Any;
        [SerializeField]
        private ActiveStateExpectation _droneBusy = ActiveStateExpectation.Any;

        public bool Active =>
            _hasCommands.Matches(_subject.HasAvailableCommands) &&
            _droneBusy.Matches(DroneCommandHandler.Instance.IsBusy);
    }
}
