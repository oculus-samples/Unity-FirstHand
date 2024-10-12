// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class AnimatorTriggerClip : PlayableAsset, ITimelineClipAsset
    {
        public AnimatorTriggerBehaviour template = new AnimatorTriggerBehaviour();
        [NonSerialized]
        public Animator binding;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AnimatorTriggerBehaviour>.Create(graph, template);
            playable.GetBehaviour().binding = binding;
            return playable;
        }
    }
}
