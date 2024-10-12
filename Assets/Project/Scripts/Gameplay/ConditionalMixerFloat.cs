// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Audio;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ConditionalMixerFloat : ActiveStateObserver
    {
        [SerializeField]
        AudioMixer _mixer;
        [SerializeField]
        string _name;
        [SerializeField]
        float _onValue = 1f;
        [SerializeField]
        float _offValue = 0f;
        [SerializeField]
        float _duration = 1f;

        private void Awake()
        {
            if (_offValue == Mathf.Infinity) _mixer.GetFloat(_name, out _offValue);
            if (_onValue == Mathf.Infinity) _mixer.GetFloat(_name, out _onValue);
        }

        private void OnEnable()
        {
            HandleActiveStateChanged();
        }

        private void OnDisable()
        {
        }

        protected override void HandleActiveStateChanged()
        {
            _mixer.GetFloat(_name, out var value);
            var target = Active ? _onValue : _offValue;
            TweenRunner.Tween(value, target, _duration, x => _mixer.SetFloat(_name, x)).SetID($"{_mixer.name}.{_name}");
        }
    }
}
