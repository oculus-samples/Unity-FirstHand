// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class FHLeverActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        FHLever _fhLever;

        [SerializeField]
        FHLever.State _state;

        public bool Active => (_fhLever.CurrentState & _state) != 0;
    }
}
