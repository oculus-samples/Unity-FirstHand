// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(OVRScreenFadeClip))]
    public class OVRScreenFadeTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<OVRScreenFadeMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
