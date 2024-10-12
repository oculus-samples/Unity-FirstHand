// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class AnimatorFloatClip : PlayableAsset, ITimelineClipAsset
    {
        public AnimatorFloatBehaviour template = new AnimatorFloatBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending | ClipCaps.Extrapolation; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AnimatorFloatBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
