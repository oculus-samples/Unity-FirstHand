// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true when the CameraLaser's state is a match
    /// </summary>
    public class CameraLaserActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private CameraLaser _cameraLaser;
        [SerializeField]
        private CameraStates _state = CameraStates.Detected;

        public bool Active => _cameraLaser.CameraStates == _state;
    }
}
