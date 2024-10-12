// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Decreases the difficulty of the turret minigame based on a fail condition
    /// </summary>
    public class ConditionalDifficultyDecrease : ActiveStateObserver
    {
        [SerializeField]
        private PlayableDirector _playableDirector;
        [SerializeField]
        private float _speed;

        private bool _hasFailedOnce;
        public event Action WhenPlayerHasFailed;

        protected override void HandleActiveStateChanged()
        {
            if (_activeState.Active && !_hasFailedOnce)
            {
                _playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(_speed);
                _hasFailedOnce = true;
                WhenPlayerHasFailed?.Invoke();
            }
        }
    }
}
