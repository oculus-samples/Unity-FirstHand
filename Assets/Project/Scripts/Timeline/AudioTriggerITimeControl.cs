// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adapter for ITimeControl and AudioTrigger
    /// Allows adding AudioTriggers to ControlTracks in timelines
    /// </summary>
    [RequireComponent(typeof(AudioTrigger))]
    public partial class AudioTriggerITimeControl : MonoBehaviour, ITimeControl
    {
        private static bool _mute;

        void ITimeControl.OnControlTimeStart()
        {
            if (_mute) return;
            GetComponent<AudioTrigger>().Play();
        }

        void ITimeControl.OnControlTimeStop()
        {
            if (this != null && TryGetComponent<AudioTrigger>(out var at)) at.Stop();
        }

        void ITimeControl.SetTime(double time) { }

        public static IDisposable Mute() { _mute = true; return new MuteContext(); }
        public static void Unmute() { _mute = false; }
        private struct MuteContext : IDisposable
        {
            void IDisposable.Dispose() => Unmute();
        }

    }

#if UNITY_EDITOR
    // HACK enables the timeline to get the duration of the audio clip
    [ExecuteInEditMode]
    partial class AudioTriggerITimeControl
    {
        void OnEnable()
        {
            if (Application.isPlaying) return;

            try
            {
                var duration = GetComponent<AudioTrigger>().MaxDuration();

                var particleSystem = gameObject.GetComponent<ParticleSystem>();
                if (!particleSystem) particleSystem = gameObject.AddComponent<ParticleSystem>();
                particleSystem.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;

                var main = particleSystem.main;
                main.duration = duration;
                main.playOnAwake = false;

                var em = particleSystem.emission;
                em.enabled = false;
                em.rateOverTime = em.rateOverDistance = 0;
            }
            catch { }
        }

        void Update()
        {
            if (Application.isPlaying) return;
            try
            {
                var duration = GetComponent<AudioTrigger>().MaxDuration();
                var particleSystem = gameObject.GetComponent<ParticleSystem>();
                particleSystem.Stop();
                var main = particleSystem.main;
                main.duration = duration;
            }
            catch{}
        }
    }
#endif
}
