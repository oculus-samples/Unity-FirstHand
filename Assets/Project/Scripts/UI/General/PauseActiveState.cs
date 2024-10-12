// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Checks if game is currently paused
    /// </summary>
    public class PauseActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        ActiveStateExpectation _isPaused = ActiveStateExpectation.True;

        public bool Active => _isPaused.Matches(PauseHandler.IsPaused);
    }
}
