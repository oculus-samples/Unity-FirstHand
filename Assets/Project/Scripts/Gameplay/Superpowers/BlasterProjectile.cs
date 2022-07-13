/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

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
        public enum Owner
        {
            Player,
            Drone
        };

        [SerializeField] private Rigidbody _rigidbody;
        public Owner ProjectileOwner { get; set; }
        private float _speed;
        public bool Lethal = true;

        public static Action<Owner> WhenAnyFire = delegate { };

        private void Awake()
        {
            Assert.IsNotNull(_rigidbody);
        }

        public void Fire(float speed)
        {
            _speed = speed;
            Lethal = true;
            WhenAnyFire(ProjectileOwner);
        }

        private void Update()
        {
            Vector3 newPosition = transform.position + (transform.forward * (_speed * Time.deltaTime));
            _rigidbody.MovePosition(newPosition);
        }
    }
}
