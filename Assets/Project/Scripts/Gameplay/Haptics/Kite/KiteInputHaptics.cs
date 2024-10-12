// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class KiteInputHaptics : MonoBehaviour
    {
        [SerializeField] private HapticClip _hapticClip;
        [SerializeField] private KiteMovementController _kite;
        [SerializeField] private KiteTail _kiteTail;
        [SerializeField] private float _amplitudePerPackage = 0.3f;
        [SerializeField] private float _frequencyPerPackage = 0.5f;

        HapticClipPlayer _left;
        HapticClipPlayer _right;

        private void Start()
        {
            _left = new HapticClipPlayer(_hapticClip);
            _right = new HapticClipPlayer(_hapticClip);
            _left.isLooping = _right.isLooping = true;
            _left.amplitude = _right.amplitude = 0;
        }

        private void OnDestroy()
        {
            _left.StopLoop();
            _left.Dispose();

            _right.StopLoop();
            _right.Dispose();
        }

        void Update()
        {
            UpdatePlayer(_left, _kite.TurnInput, Controller.Left);
            UpdatePlayer(_right, -_kite.TurnInput, Controller.Right);
        }

        private void UpdatePlayer(HapticClipPlayer player, float input, Controller left)
        {
            var turnAmplitude = Mathf.Clamp01(input) * 0.5f + 0.2f;
            var turnFrequency = turnAmplitude;

            var packageAmplitude = 1 + _kiteTail.Length * _amplitudePerPackage;
            var packageFrequency = 5 - _kiteTail.Length * _frequencyPerPackage;

            var targetAmplitude = _kite.MovementEnabled ? packageAmplitude + turnAmplitude : 0;
            var targetFrequency = _kite.MovementEnabled ? packageFrequency + turnFrequency : 0;

            var smoothedAmplitude = Mathf.MoveTowards(player.amplitude, targetAmplitude, Time.deltaTime * 4);
            var smoothedFrequency = Mathf.MoveTowards(targetFrequency, player.frequencyShift, Time.deltaTime * 4);

            player.PlayLoopWithAmplitudeAndFrequency(left, smoothedAmplitude, smoothedFrequency);
        }
    }
}
