// Copyright (c) Meta Platforms, Inc. and affiliates.

using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.8587789f, 1f, 0.759434f)]
    [TrackClipType(typeof(TextMeshProPlayableClip))]
    [TrackBindingType(typeof(TMP_Text))]
    public class TextMeshProPlayableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TextMeshProPlayableMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
