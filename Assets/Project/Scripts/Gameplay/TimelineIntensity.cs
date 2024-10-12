// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class TimelineIntensity : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector _director;

        [SerializeField]
        private float _speed = 0f;

        [SerializeField]
        private float _waveFreq = 0f;
        [SerializeField]
        private float _waveVolume = 0f;

        public float Intensity { get; set; }

        private double _lastTime = -1f;

        private void Update()
        {
            if (Mathf.Approximately(Time.timeScale, 0)) return;

            float time = _speed > 0f ? Mathf.Repeat((float)_director.time + _speed * Intensity * Time.deltaTime, 1f) : Intensity;
            float wave = Mathf.Sin(Time.time * _waveFreq) * _waveVolume;
            time += wave;

            if (time != _lastTime)
            {
                _lastTime = time;
                _director.time = Math.Max(0, Math.Min(_director.duration, time));
                _director.Evaluate();
            }
        }
    }
}
