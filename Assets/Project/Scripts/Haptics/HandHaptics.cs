// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using Oculus.Interaction.Input;
using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles hand haptics
    /// </summary>
    public class HandHaptics
    {
        public static HandHaptics Left = new HandHaptics(Haptics.Controller.Left);
        public static HandHaptics Right = new HandHaptics(Haptics.Controller.Right);

        public static HandHaptics Get(Haptics.Controller hand) => hand == Haptics.Controller.Left ? Left : Right;
        public static HandHaptics Get(IHand hand) => Get(hand.Handedness);
        public static HandHaptics Get(Handedness hand) => hand == Handedness.Left ? Left : Right;

        [SerializeField] float _amplitudeBoost = 1.0f;

        [SerializeField]
        private Haptics.Controller _hand;

        private HapticClipPlayer _player;

        private HandHaptics(Haptics.Controller hand)
        {
            _hand = hand;
            if (hand == Haptics.Controller.Left)
            {
                Application.quitting += OnApplicationQuit;
            }
        }

        public HapticClipPlayer PlayHaptic(HapticClip clip, bool loop = false, float amplitude = 1.0f, float frequencyShift = 0)
        {
            Stop();

            try
            {
                _player = new HapticClipPlayer(clip);
                _player.isLooping = loop;
                _player.frequencyShift = frequencyShift;

                var gloabl = HapticsManager.Instance ? HapticsManager.Instance.globalAmplitude : 1;
                _player.amplitude = amplitude * HapticsManager.Instance.globalAmplitude;

                _player.Play(_hand);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return _player;
        }

        public void Stop()
        {
            if (_player != null)
            {
                _player.TryStop();
                _player.Dispose();
                _player = null;
            }
        }

        private void OnApplicationQuit()
        {
            _player?.Dispose();
            _player = null;
            Haptics.Haptics.Instance.Dispose();
        }
    }
}
