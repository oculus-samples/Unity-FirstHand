// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Main component for the "BlasterProjectile" prefab. Objects with this component are created and fired with the
    /// "Blaster" component.
    /// If an object with this component hits another object with a "ProjectileHitReaction" component, the
    /// ProjectileHitReaction.OnProjectileHit() method is called. This is used for hit reactions on objects.
    /// </summary>
    public class BlasterProjectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;
        public bool FinishOnStaticCollision;
        public bool Lethal = true;

        public Owner ProjectileOwner { get; set; }
        private float _speed;
        public static Action<Owner> WhenAnyFire = delegate { };

        private void Awake()
        {
            Assert.IsNotNull(_rigidbody);
        }

        public void Fire(float speed)
        {
            gameObject.SetActive(true);
            _speed = speed;
            Lethal = true;
            WhenAnyFire(ProjectileOwner);
        }

        private void Update()
        {
            Vector3 newPosition = transform.position + (transform.forward * (_speed * Time.deltaTime));
            _rigidbody.MovePosition(newPosition);
        }

        public void Finish()
        {
            gameObject.SetActive(false);
            // TODO particle burst?
        }

        private void OnTriggerEnter(Collider other)
        {
            ProjectileHitReaction hitReaction = other.GetComponentInParent<ProjectileHitReaction>();
            if (hitReaction != null)
            {
                hitReaction.OnProjectileHit(this);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Collision: {collision.collider.name}", this);

            // hit a collider without a rigidbody, its probably a static mesh collider
            bool isStatic = collision.rigidbody == null;

            if (isStatic && FinishOnStaticCollision)
            {
                Finish();
            }
        }

        public enum Owner
        {
            Player,
            Enemy
        }
    }
}
