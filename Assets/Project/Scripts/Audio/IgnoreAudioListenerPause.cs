// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Ignores the audio listener getting paused
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class IgnoreAudioListenerPause : MonoBehaviour
    {
        void Awake() => GetComponent<AudioSource>().ignoreListenerPause = true;
    }
}
