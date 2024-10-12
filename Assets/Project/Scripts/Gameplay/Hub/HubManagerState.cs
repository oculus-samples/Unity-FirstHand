// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using static Oculus.Interaction.ComprehensiveSample.HubManager;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class HubManagerState : MonoBehaviour, IActiveState
    {
        [SerializeField, Optional] HubManager _hubManager;
        [SerializeField] ActiveStateExpectation _isAnyPlaying = ActiveStateExpectation.Any;
        [SerializeField] ActiveStateExpectation _introPlayed = ActiveStateExpectation.Any;
        [SerializeField] ActiveStateExpectation _mixedRealityPlayed = ActiveStateExpectation.Any;

        public bool Active
        {
            get
            {
                HubState state = GetState();

                return _introPlayed.Matches(state.introPlayed) &&
                            _mixedRealityPlayed.Matches(state.mixedRealityIntroPlayed) &&
                            _isAnyPlaying.Matches(_hubManager && _hubManager.IsAnyPlaying);
            }
        }

        private HubState GetState()
        {
            if (_hubManager) return _hubManager.State;

            var stateString = Store.GetString("hub.state");
            return string.IsNullOrEmpty(stateString) ? default : JsonUtility.FromJson<HubState>(stateString);
        }

        [ContextMenu("Set Intro Done")]
        private void SetIntroDone()
        {
            var state = GetState();
            state.introPlayed = true;
            Store.SetString("hub.state", JsonUtility.ToJson(state));
        }
    }
}
