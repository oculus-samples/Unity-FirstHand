// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Provides static methods for using AddExplosionForce
    /// </summary>
    public static class ExplosiveForce
    {
        static Collider[] _colliders = new Collider[128];
        static HashSet<Rigidbody> _bodies = new HashSet<Rigidbody>();

        public static event Action<Vector3, float, float, float> OnExplode;

        public static void Explode(Vector3 centerOfExplosion, float radius, float force, float upwardsModifier, bool withTorque = true)
        {
            int count = Physics.OverlapSphereNonAlloc(centerOfExplosion, radius, _colliders);
            for (int i = 0; i < count; i++)
            {
                if (GetRigidbodyOnce(_colliders[i], out var rigidbody))
                {
                    if (withTorque)
                    {
                        rigidbody.AddExplosionForce(force, centerOfExplosion, radius, upwardsModifier);
                    }
                    else
                    {
                        var rbPos = rigidbody.worldCenterOfMass;
                        var explosionDirection = rbPos - centerOfExplosion;
                        var affect = 1 - (explosionDirection.magnitude / radius);
                        rigidbody.AddForce(force * affect * explosionDirection.normalized, ForceMode.Impulse);
                    }
                }
            }
            _bodies.Clear();

            OnExplode?.Invoke(centerOfExplosion, radius, force, upwardsModifier);
        }

        public static void Explode(Vector3 start, Vector3 end, float radius, float force, float upwardsModifier)
        {
            var line = end - start;
            var lineLength = line.magnitude;
            var direction = line.normalized;

            int count = Physics.OverlapCapsuleNonAlloc(start, end, radius, _colliders);
            for (int i = 0; i < count; i++)
            {
                if (GetRigidbodyOnce(_colliders[i], out var rigidbody))
                {
                    var closestPoint = Mathf.Clamp(Vector3.Dot(rigidbody.position - start, direction), 0f, lineLength);
                    var position = start + direction * closestPoint;
                    rigidbody.AddExplosionForce(force, position, radius, upwardsModifier);
                }
            }

            _bodies.Clear();
        }

        private static bool GetRigidbodyOnce(Collider c, out Rigidbody rigidbody)
        {
            rigidbody = null;
            if (c.isTrigger) return false;

            rigidbody = c.attachedRigidbody;
            if (!rigidbody || _bodies.Contains(rigidbody)) return false;

            _bodies.Add(rigidbody);
            return rigidbody;
        }
    }

    /// <summary>
    /// Configurable settings for an explosion
    /// </summary>
    [System.Serializable]
    public struct Explosion
    {
        [SerializeField] FloatRange _radius;
        [SerializeField] FloatRange _force;
        [SerializeField] FloatRange _upwardsModifier;

        public void Explode(Vector3 position)
        {
            ExplosiveForce.Explode(position, _radius.Random(), _force.Random(), _upwardsModifier.Random());
        }

        public void Explode(Vector3 start, Vector3 end)
        {
            ExplosiveForce.Explode(start, end, _radius.Random(), _force.Random(), _upwardsModifier.Random());
        }
    }
}
