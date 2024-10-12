// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class SubtitlePlayableBehaviour : PlayableBehaviour
    {
        public bool Mute;
        public Transform Transform;
        public CharacterSubtitlePreset Preset;
        public string String;
        internal float DurationOverride;

        public override void OnPlayableCreate(Playable playable)
        {

        }
    }
}
