// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DroneMiniGameActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        DroneMiniGame _droneMiniGame;
        [SerializeField]
        IntRange _roundsPlayed = new IntRange();
        [SerializeField]
        ActiveStateExpectation _isPlaying = ActiveStateExpectation.Any;
        [SerializeField]
        ActiveStateExpectation _isTutorial = ActiveStateExpectation.Any;
        [SerializeField]
        ActiveStateExpectation _isTutorialIntroComplete = ActiveStateExpectation.Any;
        [SerializeField]
        IntRange _roundsWon = new IntRange();

        public bool Active
        {
            get
            {
                if (!_roundsPlayed.Contains(_droneMiniGame.RoundsPlayed))
                {
                    return false;
                }
                if (!_roundsWon.Contains(_droneMiniGame.RoundsWon))
                {
                    return false;
                }
                if (!_isPlaying.Matches(_droneMiniGame.IsPlaying))
                {
                    return false;
                }
                if (!_isTutorial.Matches(_droneMiniGame.IsTutorial))
                {
                    return false;
                }
                if (!_isTutorialIntroComplete.Matches(_droneMiniGame.Tutorial.IsIntroComplete))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
