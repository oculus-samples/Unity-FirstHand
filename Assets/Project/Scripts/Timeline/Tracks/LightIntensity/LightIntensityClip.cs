// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class LightIntensityClip : PlayableAsset, ITimelineClipAsset
    {
        public LightIntensityBehaviour template = new LightIntensityBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LightIntensityBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
