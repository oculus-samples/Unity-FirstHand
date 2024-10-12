// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true if the glove state matches the values set in the inspector
    /// </summary>
    public class IsGloveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        GloveBehaviour _glove;

        [SerializeField]
        ActiveStateExpectation _isWorn;

        [SerializeField]
        ActiveStateExpectation _isOpen;

        public bool Active => _isWorn.Matches(_glove.IsWorn) && _isOpen.Matches(_glove.IsOpen);
    }
}
