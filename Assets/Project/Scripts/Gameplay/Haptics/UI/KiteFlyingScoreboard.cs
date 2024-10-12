// Copyright (c) Meta Platforms, Inc. and affiliates.

using TMPro;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class KiteFlyingScoreboard : MonoBehaviour
    {
        [SerializeField]
        private KitePackageController _packageController;

        [SerializeField]
        private TextMeshProUGUI _timerText, _packageAmountText, _roundText;
        [SerializeField]
        private UICanvas _mainCanvas;
        [SerializeField]
        private UICanvas _timerCanvas;

        private void Start()
        {
            _packageController.WhenChanged += UpdateUI;
            UpdateUI();
        }

        private void OnDestroy()
        {
            _packageController.WhenChanged -= UpdateUI;
        }

        private void UpdateUI()
        {
            _mainCanvas.Show(_packageController.IsPlaying);

            if (_packageController.IsPlaying)
            {
                _packageAmountText.SetText($"{_packageController.CollectedCount.ToString("00")} / {_packageController.TargetCollectedCount.ToString("00")}");
            }
        }
    }
}
