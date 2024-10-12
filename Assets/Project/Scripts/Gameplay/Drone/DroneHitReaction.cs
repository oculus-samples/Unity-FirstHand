// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Performs an animation sequence on the drone when it is hit by a blaster projectile.
    /// </summary>
    public class DroneHitReaction : ProjectileHitReaction
    {
        [SerializeField] private Animator _actionAnimator;

        private int _knockBackParameter;

        private void Awake()
        {
            Assert.IsNotNull(_actionAnimator);
            _knockBackParameter = Animator.StringToHash("DoHitReaction");
        }

        protected override bool HandleProjectileHit(BlasterProjectile projectile)
        {
            _actionAnimator.SetTrigger(_knockBackParameter);
            return true;
        }
    }
}
