// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    [CreateAssetMenu(fileName = "CharacterPreset", menuName = "ScriptableObjects/Subtitle/CharacterPreset")]
    public class CharacterSubtitlePreset : ScriptableObject
    {
        [SerializeField]
        private Sprite _characterIcon;
        [SerializeField]
        private Color _backgroundColor, _borderColor, _textBoxColor;

        public void UpdateActiveSubtitle(Graphic icon, Rectangle background01, Rectangle background02, Rectangle border, Rectangle textBox)
        {
            icon.SetSprite(_characterIcon);

            background01.color = background02.color = _backgroundColor;
            border.color = _borderColor;
            textBox.color = _textBoxColor;
        }
    }
}
