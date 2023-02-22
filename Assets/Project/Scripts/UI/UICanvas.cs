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
using System.Threading.Tasks;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Wraps a Canvas/CanvasGroup to add basic show/hide functionality
    /// via Show and ShowAsync methods, used to show or hide UI
    /// An IUICanvasAnimator component can be added to override the default fade animation
    /// </summary>
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(CanvasGroup))]
    public sealed class UICanvas : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private bool _show = true;
        [SerializeField]
        private float _duration = 0.3f;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private IUICanvasAnimator _canvasAnimator;
        private TaskCompletionSource<bool> _canvasAnimatorTask;
        private bool _started;

        public bool Active => _show;

        private void Awake()
        {
            //instantly set the initial state
            var duration = _duration;
            _duration = 0;
            Show(!(_show = !_show));
            _duration = duration;
        }

        public async void Show(bool value)
        {
            await ShowAsync(value);
        }

        public async Task<bool> ShowAsync(bool show)
        {
            if (_show == show) return true;

            _show = show;

            EnsureReferences();
            TweenRunner.Kill(_canvasGroup);

            var hasAnimator = _canvasAnimator != null;
            if (hasAnimator && _canvasAnimatorTask != null)
            {
                var task = _canvasAnimatorTask;
                _canvasAnimatorTask = null;
                _canvasAnimator.Cancel();
                task.TrySetResult(false);
            }

            if (show) _canvas.enabled = true;
            _canvasGroup.blocksRaycasts = false;

            bool finished = true;

            if (hasAnimator && _duration > 0)
            {
                var tween = new TaskCompletionSource<bool>();
                _canvasAnimatorTask = tween;
                await Task.WhenAny(_canvasAnimator.Animate(show), tween.Task);
                bool cancelled = _canvasAnimatorTask != tween;
                if (!cancelled)
                {
                    _canvasAnimatorTask = null;
                }
                tween.TrySetResult(!cancelled);
                finished = tween.Task.Result;
            }
            else
            {
                float duration = Mathf.Abs(_canvasGroup.alpha - (show ? 1f : 0f)) * _duration;

                if (duration > 0)
                {
                    finished = await TweenRunner.Tween(_canvasGroup.alpha, show ? 1f : 0f, duration, x => _canvasGroup.alpha = x)
                        .SetID(_canvasGroup)
                        .ToTask();
                }
            }

            if (finished)
            {
                if (!hasAnimator) _canvasGroup.alpha = show ? 1f : 0f;
                if (!show) _canvas.enabled = false;
                _canvasGroup.blocksRaycasts = show;
            }

            return finished;
        }

        private void EnsureReferences()
        {
            if (_started) { return; }
            _started = true;

            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasAnimator = GetComponent<IUICanvasAnimator>();
        }
    }

    public interface IUICanvasAnimator
    {
        Task Animate(bool show);
        void Cancel();
    }
}
