// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    internal class AnimatorIntClip : PlayableAsset, ITimelineClipAsset
    {
        public AnimatorIntBehaviour template = new AnimatorIntBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Extrapolation; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AnimatorIntBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
