// Copyright (c) Meta Platforms, Inc. and affiliates.

using TMPro;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Progress the schematic as you build the drone together
    /// </summary>
    public class DroidSchematicProgressTracker : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _progressText;

        [SerializeField]
        private ReferenceActiveState[] _activeStates;

        [SerializeField]
        private GameObject _fillOne, _fillTwo, _fillThree, _fillFour, _fillFive, _fillFull;

        private int _progressAmount;
        private GameObject[] _fills;

        private void Awake()
        {
            _fills = new GameObject[] { _fillOne, _fillTwo, _fillThree, _fillFour, _fillFive, _fillFull };

            SetProgressAmount(0);
        }

        void Update()
        {
            int completeCount = CountCompletedSprites();
            if (completeCount != _progressAmount)
            {
                SetProgressAmount(completeCount);
            }
        }

        private void SetProgressAmount(int progress)
        {
            _progressAmount = progress;

            var normalizedProgress = _progressAmount / (float)_activeStates.Length;
            _progressText.text = $"{(int)(normalizedProgress * 100)}%";

            for (int i = 0; i < _fills.Length; i++)
            {
                _fills[i].SetActive(progress > i);
            }
        }

        private int CountCompletedSprites()
        {
            int completeCount = 0;
            for (int i = 0; i < _activeStates.Length; i++)
            {
                if (_activeStates[i].Active) completeCount++;
            }
            return completeCount;
        }
    }
}
