// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{

    public class PlayerHitReaction : ProjectileHitReaction
    {
        [SerializeField, Optional]
        private ReferenceActiveState _activeState;

        public GameObject hitParticlesPrefab;

        protected override bool HandleProjectileHit(BlasterProjectile projectile)
        {
            if (!_activeState.Active) return false;

            Instantiate(hitParticlesPrefab, projectile.transform.position, Quaternion.identity); //TODO pooling
            return true;
        }
    }
}
