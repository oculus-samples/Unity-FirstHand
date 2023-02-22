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

using System.Threading.Tasks;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class UICanvasCurves : MonoBehaviour, IUICanvasAnimator
    {
        [SerializeField]
        private float _showDuration = 0.5f;
        [SerializeField]
        private AnimationCurve _showCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        private float _hideDuration = 0.5f;
        [SerializeField]
        private AnimationCurve _hideCurve = AnimationCurve.Linear(0, 1, 1, 0);

        private CanvasGroup _canvasGroup;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public Task Animate(bool show)
        {
            var duration = show ? _showDuration : _hideDuration;
            var curve = show ? _showCurve : _hideCurve;
            return TweenRunner.Tween01(duration, x => _canvasGroup.alpha = curve.Evaluate(x)).SetID(this).ToTask();
        }

        public void Cancel()
        {
            TweenRunner.Kill(this);
        }
    }
}
