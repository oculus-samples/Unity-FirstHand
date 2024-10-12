// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class HandScannerTriggerArea : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private ReferenceActiveState _leftHandActive, _rightHandActive;
        public ReferenceActiveState LeftHandActive => _leftHandActive;
        public ReferenceActiveState RightHandActive => _rightHandActive;

        [SerializeField]
        private ReferenceActiveState _leftHandInZone, _rightHandInZone;
        public ReferenceActiveState LeftHandInZone => _leftHandInZone;
        public ReferenceActiveState RightHandInZone => _rightHandInZone;

        [Header("Additional")]
        [SerializeField]
        private ReferenceActiveState _checkForGesture;

        public bool Active => LeftHandActive && LeftHandInZone ||
            RightHandActive && RightHandInZone;
    }
}
