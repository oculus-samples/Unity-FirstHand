// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Base class for blaster "hit reaction" behaviors. If a "BlasterProjectile" intersects with an object that has a
    /// component deriving from this class, HandleProjectileHit() is called on the component.
    /// </summary>
    public abstract class ProjectileHitReaction : MonoBehaviour
    {
        public static event Action<ProjectileHitReaction> WhenAnyHit = delegate { };

        [SerializeField] private BlasterProjectile.Owner _projectileOwnerToReactTo;
        [SerializeField] private bool _absorbProjectile = true;
        [SerializeField] private bool _delethalizeProjectile = true;
        [SerializeField] private bool _invokeWhenHit = true;
        [SerializeField, Optional] private AudioTrigger _hitAudio = null;

        public void OnProjectileHit(BlasterProjectile projectile) => OnProjectileHit(projectile.ProjectileOwner, projectile);

        public void OnProjectileHit(BlasterProjectile.Owner owner, BlasterProjectile projectile)
        {
            if (owner != _projectileOwnerToReactTo
                || !HandleProjectileHit(projectile)) return;

            HandleProjectileHit(projectile);

            if (projectile)
            {
                if (_absorbProjectile) projectile.Finish();
                if (_delethalizeProjectile) projectile.Lethal = false;
            }

            if (_invokeWhenHit) WhenAnyHit(this);

            if (_hitAudio) _hitAudio.Play();
        }

        protected abstract bool HandleProjectileHit(BlasterProjectile projectile);
    }
}
