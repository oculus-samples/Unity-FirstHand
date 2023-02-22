/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
