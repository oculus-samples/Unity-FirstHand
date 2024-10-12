// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the color of loco's tablet suring the Street level
    /// </summary>
    public class ToggleTabletVisual : MonoBehaviour
    {
        [SerializeField]
        private Sprite _blueTablet, _redTablet;
        [SerializeField]
        private Image _tabletUI;
        [SerializeField]
        private ProgressTracker _progressMain;

        private void Update()
        {
            if (_progressMain.Progress >= 415)
                _tabletUI.sprite = _blueTablet;
            else
                _tabletUI.sprite = _redTablet;
        }
    }
}
