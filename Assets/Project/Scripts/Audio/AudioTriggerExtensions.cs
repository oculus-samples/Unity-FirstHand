// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adds fade in and fade out methods to AudioTrigger
    /// </summary>
    public static class AudioTriggerExtensions
    {
        static FieldInfo _audioClips = typeof(AudioTrigger).GetField("_audioClips", BindingFlags.Instance | BindingFlags.NonPublic);
        static FieldInfo _audioSource = typeof(AudioTrigger).GetField("_audioSource", BindingFlags.Instance | BindingFlags.NonPublic);

        public static AudioClip[] GetClips(this AudioTrigger audioTrigger)
        {
            return _audioClips.GetValue(audioTrigger) as AudioClip[];
        }

        public static AudioSource GetSource(this AudioTrigger audioTrigger)
        {
            return _audioSource.GetValue(audioTrigger) as AudioSource;
        }

        public static void Play(this AudioTrigger audioTrigger)
        {
            if (audioTrigger is AudioTriggerExtended fader) fader.PlayAudio();
            else audioTrigger.PlayAudio();
        }

        public static void PlayAudio(this AudioTrigger audioTrigger, float fadeIn = -1, bool restart = true)
        {
            if (audioTrigger.GetClips().Length == 0) return;

            var _audioSource = audioTrigger.GetComponent<AudioSource>();

            if (restart || !_audioSource.isPlaying)
            {
                audioTrigger.PlayAudio();
            }

            if (fadeIn > 0)
            {
                var endVolume = restart ? _audioSource.volume : audioTrigger.Volume;
                _audioSource.volume = 0;

                audioTrigger.StopAllCoroutines();
                audioTrigger.StartCoroutine(FadeRoutine());
                IEnumerator FadeRoutine()
                {
                    var diff = endVolume - _audioSource.volume;
                    while (_audioSource.volume < endVolume)
                    {
                        _audioSource.volume += diff * Time.deltaTime / fadeIn;
                        yield return null;
                    }
                    _audioSource.volume = endVolume;
                }
            }
        }

        public static float MaxDuration(this AudioTrigger audioTrigger)
        {
            var clips = audioTrigger.GetClips();
            if (clips.Length == 0) return 0;

            var result = clips[0].length;
            for (int i = 1; i < clips.Length; i++)
            {
                result = Mathf.Max(result, clips[i].length);
            }
            return result;
        }

        public static bool IsPlaying(this AudioTrigger audioTrigger)
        {
            var audioSource = audioTrigger.GetSource();
            return audioSource && audioSource.isPlaying;
        }

        public static void Stop(this AudioTrigger audioTrigger, float fadeOut = -1, bool stop = true)
        {
            if (audioTrigger is AudioTriggerExtended fader) fadeOut = fader.FadeOut;

            var audioSource = audioTrigger.GetComponent<AudioSource>();
            if (fadeOut <= 0)
            {
                audioSource.volume = 0;
                if (stop)
                {
                    audioSource.Stop();
                }
            }
            else
            {
                audioTrigger.StopAllCoroutines();
                audioTrigger.StartCoroutine(FadeRoutine());

                IEnumerator FadeRoutine()
                {
                    float diff = audioSource.volume;
                    while (audioSource.volume > 0)
                    {
                        audioSource.volume -= diff * Time.deltaTime / fadeOut;
                        yield return null;
                    }

                    if (stop)
                    {
                        audioSource.Stop();
                    }
                }
            }
        }
    }
}
