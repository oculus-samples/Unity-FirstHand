// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class TargetProjectileReaction : ProjectileHitReaction
    {
        [SerializeField]
        GameObject _particlePrefab;
        [SerializeField]
        HitAction _hitAction = HitAction.Destroy;

        bool _destroyed = false;

        protected override bool HandleProjectileHit(BlasterProjectile _)
        {
            if (_destroyed) { return false; }
            _destroyed = true;

            Instantiate(_particlePrefab, transform.position, transform.rotation); //TODO pooling

            if (_hitAction == HitAction.Destroy)
            {
                Destroy(gameObject);
            }
            else if (_hitAction == HitAction.Disable)
            {
                gameObject.SetActive(false);
            }

            return true;
        }

        private void OnEnable()
        {
            _destroyed = false;
        }

        enum HitAction
        {
            None,
            Disable,
            Destroy
        }
    }
}
