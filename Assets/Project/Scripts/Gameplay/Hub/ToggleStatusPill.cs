// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ToggleStatusPill : MonoBehaviour
    {
        [SerializeField]
        private Rectangle _background;
        [SerializeField]
        private Graphic _icon;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private List<StatusPillVisuals> _statusPillVisuals = new List<StatusPillVisuals>();

        public void UpdateStatusPillVisual(int i)
        {
            StatusPillVisuals statusPillVisual = _statusPillVisuals[i];

            _background.color = statusPillVisual.BackgroundColor;
            _icon.SetSprite(statusPillVisual.Sprite);
            _text.text = LocalizedText.GetUIText(statusPillVisual.String);
        }

        [System.Serializable]
        public struct StatusPillVisuals
        {
            [SerializeField]
            private Color _backgroundColor;
            public Color BackgroundColor => _backgroundColor;

            [SerializeField]
            private Sprite _sprite;
            public Sprite Sprite => _sprite;

            [SerializeField]
            private string _string;
            public string String => _string;
        }
    }
}
