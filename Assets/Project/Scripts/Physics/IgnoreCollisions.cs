// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Prevents collisions between a list of colliders
    /// Use when a rigidbody is setup/designed to intersect another
    /// </summary>
    public class IgnoreCollisions : MonoBehaviour
    {
        [SerializeField]
        List<Collider> _colliders = new List<Collider>();

        private void Reset()
        {
            GetComponentsInChildren(true, _colliders);
        }

        private void Start()
        {
            for (int i = 0; i < _colliders.Count - 1; i++)
            {
                for (int j = i + 1; j < _colliders.Count; j++)
                {
                    Physics.IgnoreCollision(_colliders[i], _colliders[j]);
                }
            }
        }
    }
}
