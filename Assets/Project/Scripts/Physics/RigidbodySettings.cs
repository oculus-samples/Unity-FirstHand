// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Applies center of mass value to rigidbody
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodySettings : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        [SerializeField, Optional]
        private Transform _centerOfMass;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_centerOfMass)
            {
                _rigidbody.centerOfMass = _rigidbody.transform.InverseTransformPoint(_centerOfMass.position);
            }
        }
    }
}
