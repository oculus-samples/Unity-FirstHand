// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// 'Damages' the player when hit by a projectile
    /// </summary>
    public class DamagePlayerOnHit : MonoBehaviour
    {
        [SerializeField]
        PlayerHealth _playerHealth;
        [SerializeField]
        ReferenceActiveState _canDamage;

        private void Start() => ProjectileHitReaction.WhenAnyHit += IncrementDroneScore;
        private void OnDestroy() => ProjectileHitReaction.WhenAnyHit -= IncrementDroneScore;

        private void IncrementDroneScore(ProjectileHitReaction hit)
        {
            if (hit is PlayerHitReaction && _canDamage)
            {
                _playerHealth.Damage();
            }
        }

    }
}
