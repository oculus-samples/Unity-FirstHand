// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Transcribes the users voice command to text
    /// </summary>
    class AppVoiceUI : MonoBehaviour
    {
        [SerializeField]
        private AppVoiceExperience _appVoiceExperience;
        [SerializeField, FormerlySerializedAs("_fullTranscriptText")]
        private TextMeshProUGUI _textLabel;

        private void Start()
        {
            _textLabel.text = string.Empty;

            var voiceEvents = _appVoiceExperience.VoiceEvents;
            voiceEvents.OnFullTranscription.AddListener(OnFullTranscription);
            voiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        }

        private void OnFullTranscription(string text)
        {
            _textLabel.SetText(text);
            Fade(0f, 2f);
        }

        private void OnPartialTranscription(string text)
        {
            _textLabel.SetText(text);
            Fade(1f, 0f);
        }

        private void Fade(float alpha, float delay)
        {
            var color = _textLabel.color;
            color.a = alpha;
            TweenRunner.Tween(_textLabel.color, color, 0.3f, x => _textLabel.color = x).Delay(delay).SetID(this);
        }
    }
}
