// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls Rectangle radius based on UIState
    /// </summary>
    public class UIStateRectangleRadius : UIStateVisual
    {
        [SerializeField]
        UIStateValues<float> _normal;
        [SerializeField]
        UIStateValues<float> _active;

        [SerializeField]
        private bool _useActive = true;
        [SerializeField]
        private float _duration = 0.2f;

        private Rectangle _rectangle;

        protected override void OnEnable()
        {
            _rectangle = GetComponent<Rectangle>();
            base.OnEnable();
        }

        protected override void UpdateVisual(IUIState uiState, bool animate)
        {
#if UNITY_EDITOR
            if (transform == null) { return; }
#endif
            var scale = (_useActive && uiState.Active ? _active : _normal).GetValue(uiState.State, -1);
            TweenRunner.Tween(_rectangle.RadiusOverride, scale, _duration, x => _rectangle.RadiusOverride = x)
                .SetID(this)
                .Skip(!animate);
        }
    }
}
