// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class OVRPassthroughEffectClip : PlayableAsset, ITimelineClipAsset
    {
        public OVRPassthroughEffectBehaviour template = new OVRPassthroughEffectBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<OVRPassthroughEffectBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
