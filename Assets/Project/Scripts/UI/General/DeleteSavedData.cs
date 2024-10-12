// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Deletes values from PlayerPrefs
    /// </summary>
    public class DeleteSavedData : MonoBehaviour
    {
        [SerializeField]
        List<HoloTableInformationPreset> _coinInfo;
        [SerializeField]
        List<ReferenceActiveState> _levels;
        [SerializeField]
        Button _button;
        [SerializeField]
        TextMeshProUGUI _percentLabel, _timeLabel;

        IEnumerator Start()
        {
            _button.onClick.AddListener(DeleteData);
            yield return null;
            UpdateUI();
        }

        private void UpdateUI()
        {
            float percent = GetPercentComplete();
            _percentLabel.text = Mathf.FloorToInt(percent * 100) + "%";

            var time = GetPlayTime();
            _timeLabel.text = time;
        }

        private string GetPlayTime()
        {
            float seconds = Store.GetFloat("play_time");
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString(@"hh\:mm");
        }

        private void DeleteData()
        {
            Store.DeleteAll();
            UpdateUI();
        }

        public float GetPercentComplete()
        {
            float coinsCollected = 0;
            float coinsTotal = 0;

            for (int i = 0; i < _coinInfo.Count; i++)
            {
                var l = _coinInfo[i];
                coinsCollected += l.CoinsCollectedAmount;
                coinsTotal += l.CoinsTotalAmount;
            }

            var coinsPercent = coinsCollected / coinsTotal;

            float levels = 0;
            for (int i = 0; i < _levels.Count; i++)
            {
                if (_levels[i])
                {
                    levels++;
                }
            }

            var levelsPercent = levels / _levels.Count;

            return levelsPercent * 0.7f + coinsPercent * 0.3f;
        }
    }
}
