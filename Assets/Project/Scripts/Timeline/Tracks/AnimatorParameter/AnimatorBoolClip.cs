// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    internal class AnimatorBoolClip : PlayableAsset, ITimelineClipAsset
    {
        public AnimatorBoolBehaviour template = new AnimatorBoolBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Extrapolation; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AnimatorBoolBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
