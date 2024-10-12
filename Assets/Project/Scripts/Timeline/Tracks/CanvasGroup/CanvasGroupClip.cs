// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class CanvasGroupClip : PlayableAsset, ITimelineClipAsset
    {
        public CanvasGroupBehaviour Template = new CanvasGroupBehaviour();

        public ClipCaps clipCaps => ClipCaps.Blending | ClipCaps.Extrapolation;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<CanvasGroupBehaviour>.Create(graph, Template);
        }
    }
}
