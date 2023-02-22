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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Was = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Main component for the "Blaster" prefab. Creates a pool of "BlasterProjectile" objects and fires them from
    /// _projectileFireTransform at speed _projectileSpeed.
    /// </summary>
    public class Blaster : MonoBehaviour, IFireable
    {
        [SerializeField, Optional] private Transform _projectileFireTransform;
        [SerializeField] private BlasterProjectile _projectilePrefab;
        [SerializeField] private int _projectilePoolSize = 10;
        [SerializeField, Optional] private AudioTrigger _audio;
        private List<GameObject> _projectiles;
        private int _nextProjectileIndex;

        [SerializeField] private float _projectileSpeed = 100f;
        [SerializeField, Was("projectileOwner")] private BlasterProjectile.Owner _projectileOwner;

        private void Awake()
        {
            Assert.IsNotNull(_projectilePrefab);

            _projectiles = new List<GameObject>(_projectilePoolSize);
            for (int i = 0; i < _projectilePoolSize; ++i)
            {
                GameObject projectileObject = Instantiate(_projectilePrefab.gameObject);
                _projectiles.Add(projectileObject);
                projectileObject.SetActive(false);

                BlasterProjectile projectile = projectileObject.GetComponent<BlasterProjectile>();
                projectile.ProjectileOwner = _projectileOwner;
            }

            if (!_projectileFireTransform)
            {
                _projectileFireTransform = transform;
            }
        }

        /// <summary>
        /// Gets a projectile from the pool, enables it, transforms it to _projectileFireTransform, and calls the
        /// Fire() method on the projectile while will start it moving.
        /// </summary>
        public void Fire()
        {
            GameObject projectileObject = _projectiles[_nextProjectileIndex];
            _nextProjectileIndex = (_nextProjectileIndex + 1) % _projectilePoolSize;

            projectileObject.SetActive(false);
            projectileObject.transform.SetPose(_projectileFireTransform.GetPose());
            projectileObject.SetActive(true);

            BlasterProjectile projectile = projectileObject.GetComponent<BlasterProjectile>();
            projectile.Fire(_projectileSpeed);

            if (_audio)
            {
                _audio.PlayAudio();
            }
        }
    }
}
