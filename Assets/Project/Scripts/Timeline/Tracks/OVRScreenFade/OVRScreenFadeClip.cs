// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class OVRScreenFadeClip : PlayableAsset, ITimelineClipAsset
    {
        public OVRScreenFadeBehaviour template = new OVRScreenFadeBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending | ClipCaps.Extrapolation; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<OVRScreenFadeBehaviour>.Create(graph, template);
            OVRScreenFadeBehaviour clone = playable.GetBehaviour();
            return playable;
        }
    }
}
