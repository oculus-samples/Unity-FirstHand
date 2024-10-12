// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DroneCommandHandlerActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        DroneCommandHandler _droneCommandHandler;

        [SerializeField]
        ActiveStateExpectation _isBusy = ActiveStateExpectation.Any;
        [SerializeField]
        ActiveStateExpectation _isConfused = ActiveStateExpectation.Any;
        [SerializeField]
        ActiveStateExpectation _hasCommands = ActiveStateExpectation.Any;

        public bool Active =>
            _isBusy.Matches(_droneCommandHandler.IsBusy) &&
            _hasCommands.Matches(_droneCommandHandler.HasCommands());
    }
}
