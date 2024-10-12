// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class TimelineSpeed : MonoBehaviour
    {
        [SerializeField]
        private double _speed = 1;

        private PlayableDirector _pd;

        public double Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                UpdateSpeed();
            }
        }

        private void Awake()
        {
            _pd = GetComponent<PlayableDirector>();
            _pd.played += UpdateSpeed;
        }

        private void UpdateSpeed(PlayableDirector obj) => UpdateSpeed();

        private void UpdateSpeed()
        {
            if (Application.isPlaying && _pd.playableGraph.IsValid())
            {
                _pd.playableGraph.GetRootPlayable(0).SetSpeed(_speed);
            }
        }
    }
}
