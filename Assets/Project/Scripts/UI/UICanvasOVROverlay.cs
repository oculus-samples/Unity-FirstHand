// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Threading.Tasks;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Tweens the OVR OVerlay as if it were a UI Canvas
    /// </summary>
    public class UICanvasOVROverlay : MonoBehaviour, IUICanvasAnimator
    {
        float _alpha = 0f;
        public float MaxAlpha = 1f;
        private OVROverlay _overlay;

        public Task Animate(bool show) => TweenRunner.Tween(_alpha, show ? 1 : 0, 0.2f, x => _alpha = x).SetID(this).ToTask();

        public void Cancel() => TweenRunner.Kill(this);

        void Update() => UpdateOverlay();
        void LateUpdate() => UpdateOverlay();

        private void UpdateOverlay()
        {
            _overlay ??= GetComponentInChildren<OVROverlay>();
            if (_overlay)
            {
                _overlay.noDepthBufferTesting = true;
                _overlay.overridePerLayerColorScaleAndOffset = true;
                _overlay.colorScale = new Vector4(1, 1, 1, _alpha * MaxAlpha);
            }
        }
    }
}
