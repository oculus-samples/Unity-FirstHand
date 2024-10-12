// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Collectable coin UI controller
    /// </summary>
    public class CoinTrackerUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text _counterDisplayCurrent;

        [SerializeField]
        TMP_Text _counterDisplayMax;

        [SerializeField]
        HoloTableInformationPreset _holoTableInformationPreset;

        [SerializeField]
        List<HoloTableInformationPreset> _levels = new List<HoloTableInformationPreset>();

        private void OnEnable()
        {
            var total = 0;
            _levels.ForEach(x => total += x.CoinsTotalAmount);
            if (_holoTableInformationPreset) total += _holoTableInformationPreset.CoinsTotalAmount;
            _counterDisplayMax.text = total.ToString("00");

            Store.WhenChanged += UpdateUI;
            UpdateUI();
        }

        private void OnDisable()
        {
            Store.WhenChanged -= UpdateUI;
        }

        private void UpdateUI()
        {
            var collected = 0;
            _levels.ForEach(x => collected += x.CoinsCollectedAmount);
            if (_holoTableInformationPreset) collected += _holoTableInformationPreset.CoinsCollectedAmount;
            _counterDisplayCurrent.text = collected.ToString("00");
        }
    }
}
