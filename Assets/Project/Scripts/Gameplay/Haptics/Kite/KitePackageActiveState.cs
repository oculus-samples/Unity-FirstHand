// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class KitePackageActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        KitePackageController _packageController;

        [SerializeField]
        ActiveStateExpectation _playing = ActiveStateExpectation.Any;
        [SerializeField]
        ActiveStateExpectation _collectedAll = ActiveStateExpectation.Any;
        [SerializeField]
        FloatRange _gamesCompleted = FloatRange.Infinity;
        [SerializeField]
        FloatRange _round = FloatRange.Infinity;
        [SerializeField]
        FloatRange _collectedCount = FloatRange.Infinity;

        public bool Active
        {
            get
            {
                bool collectedAll = _packageController.CollectedCount == _packageController.TargetCollectedCount;
                if (!_collectedAll.Matches(collectedAll)) return false;
                if (!_collectedCount.Contains(_packageController.CollectedCount)) return false;
                if (!_round.Contains(_packageController.Round)) return false;
                if (!_playing.Matches(_packageController.IsPlaying)) return false;
                if (!_gamesCompleted.Contains(_packageController.GamesCompleted)) return false;

                return true;
            }
        }
    }
}
