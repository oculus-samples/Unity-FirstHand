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

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Performs a raycast from _rayOrigin in Update and positions _target where the ray hits with an optional offset
    /// </summary>
    public class RaycastedPoser : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform _rayOrigin;
        [SerializeField]
        private float _positionOffset = 0.02f;
        [SerializeField]
        private OffsetMode _offsetMode;

        private bool _lastRayHit;
        public bool Active => _lastRayHit;

        private void Update()
        {
            transform.SetPose(_rayOrigin.GetPose());

            var ray = new Ray(transform.position, transform.forward);
            _lastRayHit = Physics.Raycast(ray, out var hit);

            if (_lastRayHit)
            {
                Vector3 offsetDirection = _offsetMode == OffsetMode.AlongNormal ? hit.normal : -ray.direction;
                Pose pose = new Pose(hit.point + offsetDirection * _positionOffset, Quaternion.LookRotation(hit.normal, transform.up));
                _target.SetPose(pose);
            }
        }

        private enum OffsetMode
        {
            AlongRay,
            AlongNormal,
        }
    }
}
