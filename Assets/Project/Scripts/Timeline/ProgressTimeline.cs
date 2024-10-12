// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Maps the value of a ProgressTracker onto a timeline
    /// </summary>
    [RequireComponent(typeof(PlayableDirector))]
    public class ProgressTimeline : MonoBehaviour
    {
        [SerializeField]
        private ProgressTracker _progress;
        [SerializeField]
        private float _normalizer = 5;

        private PlayableDirector _director;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            _progress.WhenChanged += TweenTimeline;

            UpdateTimeline(Mode.Instant);

            void TweenTimeline() => UpdateTimeline(Mode.Tween);
        }

        private void UpdateTimeline(Mode mode)
        {
            TweenRunner.Kill(this);

            float normalizedProgress = Mathf.Clamp01(_progress.Progress / _normalizer);
            double time = _director.duration * normalizedProgress;

            if (time != _director.time)
            {
                if (mode == Mode.Tween)
                {
                    TweenRunner.Tween(_director.time, time, Math.Abs(time - _director.time), this.SetDirectorTime).SetID(this);
                }
                else
                {
                    SetDirectorTime(time);
                }
            }
        }

        private void SetDirectorTime(double time)
        {
            _director.time = Math.Min(time, _director.duration - double.Epsilon);
            _director.DeferredEvaluate();
        }

        private enum Mode
        {
            Instant,
            Tween
        }
    }
}
