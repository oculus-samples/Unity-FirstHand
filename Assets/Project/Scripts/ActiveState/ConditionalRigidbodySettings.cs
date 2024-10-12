// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Conditional check for rigidbody settings
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ConditionalRigidbodySettings : MonoBehaviour
    {
        [SerializeField] ReferenceActiveState _useGravity = ReferenceActiveState.Optional();
        [SerializeField] ReferenceActiveState _isKinematic = ReferenceActiveState.Optional();
        [SerializeField] ReferenceActiveState _freezeRotation = ReferenceActiveState.Optional();

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_useGravity.HasReference && _rigidbody.useGravity != _useGravity) _rigidbody.useGravity = _useGravity;
            if (_isKinematic.HasReference && _rigidbody.isKinematic != _isKinematic) _rigidbody.isKinematic = _isKinematic;
            if (_freezeRotation.HasReference && _rigidbody.freezeRotation != _freezeRotation) _rigidbody.freezeRotation = _freezeRotation;
        }
    }
}
