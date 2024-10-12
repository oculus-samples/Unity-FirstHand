// Copyright (c) Meta Platforms, Inc. and affiliates.

using TMPro;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls the behaviour of the drone scoreboard
    /// </summary>
    public class DroneScoreboardUI : MonoBehaviour
    {
        [SerializeField] DroneMiniGame _miniGame;
        [SerializeField] TMP_Text _playerScoreLabel;
        [SerializeField] TMP_Text _droneScoreLabel;
        [SerializeField] TMP_Text _timeLabel;
        [SerializeField] TMP_Text _droneAccuracyLabel;
        [SerializeField] TMP_Text _playerAccuracyLabel;

        private void Awake()
        {
            _miniGame.WhenChanged += UpdateUI;
        }

        private void OnDestroy()
        {
            _miniGame.WhenChanged -= UpdateUI;
        }

        private void UpdateUI()
        {
            _playerScoreLabel.text = _miniGame.PlayerScore.ToString();
            _droneScoreLabel.text = _miniGame.DroneScore.ToString();
            _playerAccuracyLabel.text = ToPercent(_miniGame.PlayerAccuracy);
            _droneAccuracyLabel.text = ToPercent(_miniGame.DroneAccuracy);
            _timeLabel.text = _miniGame.TimeLeft.ToString();
        }

        private static string ToPercent(float f)
        {
            if (f < 0) { return "-"; }
            return Mathf.RoundToInt(f * 100) + "%";
        }

    }
}
