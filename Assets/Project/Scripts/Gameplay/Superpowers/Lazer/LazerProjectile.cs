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

using System.Collections;
using UnityEngine;
using Was = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class LazerProjectile : ActiveStateObserver
    {
        [SerializeField] Transform _target;
        [SerializeField] GameObject _effect;
        [SerializeField] float _rayCastDelay = 0.1f;
        [SerializeField] float _fadeOutTime = 0.1f;
        [SerializeField] float _delayBetweenShots = 0.5f;
        [SerializeField] AudioTrigger _chargeUp;
        [SerializeField, Optional] AudioTrigger _chargeUpComplete;

        protected override void Update()
        {
            base.Update();

            Vector3 endPos = Vector3.Lerp(_target.position, transform.position + transform.forward * 20, 5 * Time.deltaTime);
            _target.position = endPos;
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(FireRoutine());
            IEnumerator FireRoutine()
            {
                while (isActiveAndEnabled)
                {
                    if (!Active || !isActiveAndEnabled) { yield return null; continue; }

                    // refresh effects
                    _effect.SetActive(false);
                    _effect.SetActive(true);
                    _target.gameObject.SetActive(false);
                    _target.gameObject.SetActive(true);

                    _chargeUp.PlayAudio();

                    // wait for the charge up
                    yield return new WaitForSeconds(_rayCastDelay);

                    if (_chargeUpComplete) _chargeUpComplete.PlayAudio();

                    BlasterProjectile.WhenAnyFire(BlasterProjectile.Owner.Player);

                    yield return new WaitForSeconds(_fadeOutTime);

                    _target.gameObject.SetActive(false);
                    _effect.SetActive(false);

                    yield return new WaitForSeconds(_delayBetweenShots);
                }
            }
        }

        protected override void HandleActiveStateChanged()
        {
        }
    }

    public interface IFireable
    {
        void Fire();
    }
}
