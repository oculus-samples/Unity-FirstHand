// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays haptic clip based on distance
    /// </summary>
    public class DistanceHapticSource : MonoBehaviour, IDistanceHapticSource
    {
        public Vector3 Position => transform.position;

        [SerializeField] HapticClip _clip;
        public HapticClip Clip
        {
            get
            {
                return _clip;
            }
            set
            {
                _clip = value;
            }
        }

        [SerializeField] bool _loop;
        public bool Loop => _loop;

        [SerializeField] float _amplitude;
        public float Amplitude => _amplitude;

        [SerializeField] float _frequency;
        public float Frequency => _frequency;

        public event Action WhenHaptics;

        internal void Play()
        {
            WhenHaptics?.Invoke();
        }
    }

    public interface IDistanceHapticSource
    {
        public Vector3 Position { get; }
        public HapticClip Clip { get; }
        public bool Loop { get; }
        public float Amplitude { get; }
        public float Frequency { get; }

        public event Action WhenHaptics;
    }

    public static class HapticExtensions
    {
        static Dictionary<HapticClip, float> _durations = new Dictionary<HapticClip, float>();

        static HashSet<HapticClipPlayer> _playingLoops = new HashSet<HapticClipPlayer>();

        public static float GetDuration(this HapticClip clip)
        {
            if (_durations.TryGetValue(clip, out var duration)) return duration;

            var json = clip.json;
            var time = @"""time"":\s*(\d+\.\d+)";
            var match = Regex.Match(json, time, RegexOptions.RightToLeft);

            _durations[clip] = duration = match.Success ? float.Parse(match.Groups[1].Value) : 0.5f;
            return duration;
        }

        public static bool IsPlayingLoop(this HapticClipPlayer hapticClipPlayer)
        {
            return _playingLoops.Contains(hapticClipPlayer);
        }

        public static void PlayLoop(this HapticClipPlayer hapticClipPlayer, Controller hand)
        {
            if (!IsPlayingLoop(hapticClipPlayer))
            {
                _playingLoops.Add(hapticClipPlayer);
                hapticClipPlayer.isLooping = true;
                hapticClipPlayer.Play(hand);
            }
        }

        public static void SetLoopPlaying(this HapticClipPlayer hapticClipPlayer, Controller hand, bool play)
        {
            if (IsPlayingLoop(hapticClipPlayer) != play)
            {
                if (play) PlayLoop(hapticClipPlayer, hand);
                else StopLoop(hapticClipPlayer);
            }
        }

        public static void StopLoop(this HapticClipPlayer hapticClipPlayer)
        {
            if (IsPlayingLoop(hapticClipPlayer))
            {
                _playingLoops.Remove(hapticClipPlayer);
                TryStop(hapticClipPlayer);
            }
        }

        public static void PlayLoopWithAmplitudeAndFrequency(this HapticClipPlayer player, Controller hand, float amplitude, float freq = -2)
        {
            player.frequencyShift = Mathf.Clamp(freq, -1, 1);
            player.amplitude = amplitude;
            bool shouldPlay = player.amplitude > 0.001f;
            player.SetLoopPlaying(hand, shouldPlay);
        }

        public static bool TryStop(this HapticClipPlayer hapticClipPlayer)
        {
            try
            {
                hapticClipPlayer.Stop();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    [System.Serializable]
    public struct ReferenceDistanceHapticSource : ISerializationCallbackReceiver, IDistanceHapticSource
    {
        [SerializeField, Interface(typeof(IDistanceHapticSource))]
        private MonoBehaviour _hapticSource;
        public IDistanceHapticSource HapticSource { get; private set; }

        public Vector3 Position => HapticSource.Position;

        public bool HasValue => _hapticSource != null;

        public HapticClip Clip => HapticSource.Clip;

        public bool Loop => HapticSource.Loop;

        public float Amplitude => HapticSource.Amplitude;

        public float Frequency => HapticSource.Frequency;

        public event Action WhenHaptics
        {
            add => HapticSource.WhenHaptics += value;
            remove => HapticSource.WhenHaptics -= value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (HapticSource == null || (HapticSource as MonoBehaviour) == null)
            {
                if (_hapticSource && _hapticSource is IDistanceHapticSource source)
                {
                    HapticSource = source;
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}
