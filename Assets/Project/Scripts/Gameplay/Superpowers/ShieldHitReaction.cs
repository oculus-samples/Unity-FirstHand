// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Hit reaction for the player's shield. When a blaster projectile hits an object with this component, the
    /// projectile's velocity is reflected around the forward vector of this object's transform.
    /// </summary>
    public class ShieldHitReaction : ProjectileHitReaction
    {
        protected override bool HandleProjectileHit(BlasterProjectile projectile)
        {
            Vector3 normal = transform.forward;
            Vector3 newForward = Vector3.Reflect(projectile.transform.forward, normal);
            projectile.transform.forward = newForward;
            return false;
        }
    }
}
