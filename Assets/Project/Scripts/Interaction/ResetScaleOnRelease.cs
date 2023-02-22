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
    /// Tweens a Grabbable back to it's initial scale when it's released
    /// </summary>
    [RequireComponent(typeof(Grabbable))]
    public class ResetScaleOnRelease : MonoBehaviour
    {
        private Grabbable _grabbable;
        private Vector3 _initialScale;

        private void Awake()
        {
            _grabbable = GetComponent<Grabbable>();
            _initialScale = transform.localScale;
            _grabbable.WhenPointerEventRaised += HandlePointerRaised;
        }

        private void OnDestroy()
        {
            _grabbable.WhenPointerEventRaised -= HandlePointerRaised;
        }

        private void HandlePointerRaised(PointerArgs obj)
        {
            if (obj.PointerEvent != PointerEvent.Unselect || _grabbable.SelectingPointsCount > 0) { return; }

            var startScale = transform.localScale;
            TweenRunner.Tween01(0.5f, x => transform.localScale = Vector3.Lerp(startScale, _initialScale, x));
        }
    }
}
