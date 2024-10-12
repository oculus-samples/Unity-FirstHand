// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true if any part of the rigidbody is inside the zone
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class IsRigidbodyInZone : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private List<Collider> _zones = new();
        [SerializeField, Tooltip(
            "When true the point tyested will be the rigidbodies center of mass, " +
            "otherwise the closest point to each collider will be tested")]
        bool _useCenterOfMass = false;

        private Rigidbody _rigidbody;

        void Awake() => _rigidbody = GetComponent<Rigidbody>();

        public bool Active => _zones.TrueForAny(ColliderIntersectsRigidbody);

        private bool ColliderIntersectsRigidbody(Collider collider)
        {
            bool enabled = collider.enabled && collider.gameObject.activeInHierarchy;
            if (!enabled) return false;

            Vector3 point = _useCenterOfMass ? _rigidbody.worldCenterOfMass : _rigidbody.ClosestPointOnBounds(collider.bounds.center);
            return collider.IsPointWithinCollider(point);
        }
    }
}
