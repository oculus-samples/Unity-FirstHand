// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Voice;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns value based on wether the sent data matches the expectation
    /// </summary>
    public class AppVoiceExperienceActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private AppVoiceExperience _appVoiceExperience;
        [SerializeField]
        private ActiveStateExpectation _sendingRequest = ActiveStateExpectation.Any;

        private bool _sending;

        public bool Active => _sendingRequest.Matches(_sending);

        private void Awake()
        {
            _appVoiceExperience.VoiceEvents.OnSend.AddListener(x => _sending = true);
            _appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(() => _sending = false);
        }
    }
}
