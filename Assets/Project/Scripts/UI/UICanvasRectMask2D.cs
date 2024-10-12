// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Tweens a RectMask2D as if it were a UI Canvas
    /// </summary>
    public class UICanvasRectMask2D : MonoBehaviour, IUICanvasAnimator
    {
        [SerializeField]
        private float _showDuration;
        [SerializeField, Tooltip("Left, bottom right top")]
        private Vector4 _showPadding;

        [SerializeField]
        private float _hideDuration;
        [SerializeField, Tooltip("Left, bottom right top")]
        private Vector4 _hidePadding;

        private RectMask2D _rectMask2D;

        void Awake()
        {
            _rectMask2D = GetComponent<RectMask2D>();
        }

        void Start()
        {
            var shown = GetComponent<UICanvas>().IsShown;
            _rectMask2D.padding = shown ? _showPadding : _hidePadding;
        }

        public Task Animate(bool show)
        {
            var duration = show ? _showDuration : _hideDuration;
            var start = _rectMask2D.padding;
            var end = show ? _showPadding : _hidePadding;
            return TweenRunner.Tween01(duration, x => _rectMask2D.padding = Vector4.Lerp(start, end, x)).SetID(this).IgnoreTimeScale(true).ToTask();
        }

        public void Cancel()
        {
            TweenRunner.Kill(this);
        }
    }
}
