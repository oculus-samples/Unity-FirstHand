// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adapts DroneVoice Controller to IActiveState
    /// </summary>
    public class DroneVoiceControllerActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private DroneVoiceController _droneVoiceController;
        [SerializeField]
        private ActiveStateExpectation _isListening = ActiveStateExpectation.Any;

        public bool Active => _isListening.Matches(_droneVoiceController.IsListening);
    }
}
