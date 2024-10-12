// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class UIStateRectTransform : UIStateVisual
    {
        [SerializeField]
        private UITransformSet _onRect, _offRect;
        [SerializeField]
        private bool _useActive = true;
        [SerializeField]
        private float _duration = 0.1f;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        protected override void UpdateVisual(IUIState uiState, bool animate)
        {
            var rect = (_useActive && uiState.Active ? _onRect : _offRect).GetRectForState(uiState.State);
            TweenRunner.Tween(_rectTransform.localPosition, rect.localPosition, _duration, x => _rectTransform.localPosition = x)
                .IgnoreTimeScale()
                .SetID(this)
                .Skip(!animate);
        }

        [System.Serializable]
        public struct UITransformSet
        {
            [SerializeField]
            private RectTransform _rect;

            public RectTransform GetRectForState(UIStates state)
            {
                switch (state)
                {
                    default: return _rect;
                }
            }
        }
    }
}
