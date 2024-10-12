// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.7979604f, 0.9056604f, 0.6279815f)]
    [TrackClipType(typeof(SubtitlePlayableClip))]
    public class SubtitlePlayableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<SubtitlePlayableMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
