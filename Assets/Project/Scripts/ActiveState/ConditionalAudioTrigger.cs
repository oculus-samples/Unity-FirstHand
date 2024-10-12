// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays/Stops an AudioTrigger when the active state becomes true/false
    /// e.g. When linked to a PlayerInZone can start playing an ambient zone for the zone that stops when the player exits
    /// </summary>
    public class ConditionalAudioTrigger : ActiveStateObserver
    {
        [SerializeField]
        private AudioTrigger[] _audioTriggers;

        protected override void Reset()
        {
            base.Reset();
            if (TryGetComponent<AudioTrigger>(out var at))
            {
                _audioTriggers = new AudioTrigger[] { at };
            }
        }

        protected override void HandleActiveStateChanged()
        {
            if (Active)
            {
                foreach (AudioTrigger audiotrigger in _audioTriggers)
                {
                    audiotrigger.Play();
                }
            }
            else
            {
                foreach (AudioTrigger audiotrigger in _audioTriggers)
                {
                    if (audiotrigger.Loop)
                    {
                        audiotrigger.Stop();
                    }
                }
            }
        }
    }
}
