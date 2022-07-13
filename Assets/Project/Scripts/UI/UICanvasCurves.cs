/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
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
