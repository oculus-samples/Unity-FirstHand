// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class CollidersIntersectActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField] private Collider _colliderA;
        [SerializeField] private Collider _colliderB;

        public bool Active
        {
            get
            {
                bool bothEnabled = IsEnabled(_colliderA) && IsEnabled(_colliderB);

                return bothEnabled && Physics.ComputePenetration(
                    _colliderB, _colliderB.transform.position, _colliderB.transform.rotation,
                    _colliderA, _colliderA.transform.position, _colliderA.transform.rotation,
                    out var _, out var __);

                bool IsEnabled(Collider c) => c.enabled && c.gameObject.activeInHierarchy;
            }
        }
    }
}
