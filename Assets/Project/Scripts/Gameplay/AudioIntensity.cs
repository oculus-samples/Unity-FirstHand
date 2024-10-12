// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class AudioIntensity : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audio;
        [SerializeField]
        private Vector2 _volumeRange = Vector2.up;
        [SerializeField]
        private Vector2 _pitchRange = Vector2.up;
        [SerializeField]
        private float _speed = 1f;

        public float Intensity { get; set; }

        private void Reset()
        {
            _audio = GetComponent<AudioSource>();
        }

        public void StartAudio()
        {
            _audio.volume = _volumeRange.x;
            _audio.pitch = _volumeRange.y;
        }

        public void EndAudio()
        {
            _audio.volume = _volumeRange.x;
            _audio.pitch = _volumeRange.y;
            _audio.Pause();
        }

        private void Update()
        {
            float desiredVolume = Mathf.Lerp(_volumeRange.x, _volumeRange.y, Intensity);
            _audio.volume = Mathf.MoveTowards(_audio.volume, desiredVolume, Time.deltaTime * _speed);

            float desiredPitch = Mathf.Lerp(_pitchRange.x, _pitchRange.y, Intensity);
            _audio.pitch = Mathf.MoveTowards(_audio.pitch, desiredPitch, Time.deltaTime * _speed);

            if (_audio.volume == 0f && _audio.isPlaying) _audio.Pause();
            else if (_audio.volume > 0f && !_audio.isPlaying) _audio.Play();
        }
    }
}
