// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true if seated mode is enabled
    /// </summary>
    public class SeatedModeActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        ActiveStateExpectation _isOn = ActiveStateExpectation.True;

        public bool Active => _isOn.Matches(SeatedMode.IsOn);
    }
}
