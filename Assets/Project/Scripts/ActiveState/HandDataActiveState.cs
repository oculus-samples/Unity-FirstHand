// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true based on expectations of hand values
    /// </summary>
    public class HandDataActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private IHandDataReference _hand;
        [SerializeField]
        private ActiveStateExpectation _isConnected = ActiveStateExpectation.Any;
        [SerializeField]
        private ActiveStateExpectation _isTracked = ActiveStateExpectation.Any;
        [SerializeField]
        private ActiveStateExpectation _isHighConfidence = ActiveStateExpectation.Any;
        [SerializeField]
        private ActiveStateExpectation _isDominantHand = ActiveStateExpectation.Any;

        public bool Active
        {
            get
            {
                var data = _hand.GetData();
                return
                    _isTracked.Matches(data.IsTracked) &&
                    _isConnected.Matches(data.IsConnected) &&
                    _isDominantHand.Matches(data.IsDominantHand) &&
                    _isHighConfidence.Matches(data.IsHighConfidence);
            }
        }
    }
}
