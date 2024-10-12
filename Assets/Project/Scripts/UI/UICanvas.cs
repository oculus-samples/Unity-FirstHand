// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Wraps a Canvas/CanvasGroup to add basic show/hide functionality
    /// via Show and ShowAsync methods, used to show or hide UI
    /// An IUICanvasAnimator component can be added to override the default fade animation
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UICanvas : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private bool _show = true;
        [SerializeField]
        private float _duration = 0.3f;
        [SerializeField, Tooltip("Controls if the canvas enabled/gameobject active state should be controlled by this")]
        private DisableMode _disableMethod = DisableMode.Canvas;

        Canvas _canvas;
        CanvasGroup _canvasGroup;
        bool _hasGraphicRaycaster;
        GraphicRaycaster _graphicRaycaster;
        IUICanvasAnimator _canvasAnimator;
        TaskCompletionSource<bool> _canvasAnimatorTask;
        private bool _started;

        bool IActiveState.Active => _show;

        public event Action onChange;

        public float Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public bool IsShown
        {
            get => _show;
            set => Show(value);
        }

        private void Reset() => RequireCanvas();

        private void Awake()
        {
            //instantly set the initial state
            var duration = _duration;
            _duration = 0;
            Show(!(_show = !_show));
            _duration = duration;
        }

        void OnDestroy()
        {
            TweenRunner.Kill(_canvasGroup);
        }

        public async void Show(bool value)
        {
            await ShowAsync(value);
        }

        public async Task<bool> ShowAsync(bool show)
        {
            if (_show == show) return true;

            _show = show;

            if (_started) onChange?.Invoke();

            //Task has completed
            if (Application.isPlaying == false)
                return true;

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

            if (show && (_disableMethod & DisableMode.Canvas) != 0) _canvas.enabled = true;
            if (show && (_disableMethod & DisableMode.GameObject) != 0) _canvas.gameObject.SetActive(true);

            _canvasGroup.blocksRaycasts = false;
            if (_hasGraphicRaycaster) _graphicRaycaster.enabled = false;

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
                        .IgnoreTimeScale()
                        .SetID(_canvasGroup)
                        .ToTask();
                }
            }

            if (finished)
            {
                if (!hasAnimator) _canvasGroup.alpha = show ? 1f : 0f;
                if (!show && (_disableMethod & DisableMode.Canvas) != 0) _canvas.enabled = false;
                if (!show && (_disableMethod & DisableMode.GameObject) != 0) _canvas.gameObject.SetActive(false);
                if (_hasGraphicRaycaster) _graphicRaycaster.enabled = show;
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
            _hasGraphicRaycaster = TryGetComponent(out _graphicRaycaster);
        }

        /// <summary>
        /// An alternative to RequireComponent(typeof(Canvas)) that will add a a canvas component but also add a GraphicRaycaster if needed
        /// </summary>
        private void RequireCanvas()
        {
            if (TryGetComponent<Canvas>(out var _)) { return; }

            var parentCanvas = GetComponentInParent<Canvas>();
            var parentRaycaster = parentCanvas ? parentCanvas.GetComponent<GraphicRaycaster>() : null;

            gameObject.AddComponent<Canvas>();
            if (parentRaycaster)
            {
                var newRaycaster = gameObject.AddComponent<GraphicRaycaster>();
                newRaycaster.blockingMask = parentRaycaster.blockingMask;
                newRaycaster.blockingObjects = parentRaycaster.blockingObjects;
                newRaycaster.ignoreReversedGraphics = parentRaycaster.ignoreReversedGraphics;
            }
        }

        [Flags]
        public enum DisableMode
        {
            Canvas = 1 << 0,
            GameObject = 1 << 1,
        }
    }

    public interface IUICanvasAnimator
    {
        Task Animate(bool show);
        void Cancel();
    }
}
