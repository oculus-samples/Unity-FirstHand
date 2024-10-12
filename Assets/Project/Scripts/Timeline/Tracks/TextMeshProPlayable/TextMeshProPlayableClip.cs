// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class TextMeshProPlayableClip : PlayableAsset, ITimelineClipAsset
    {
        public TextMeshProPlayableBehaviour template = new TextMeshProPlayableBehaviour();

        [TextArea(5, 10)]
        public string String;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TextMeshProPlayableBehaviour>.Create(graph, template);
            TextMeshProPlayableBehaviour clone = playable.GetBehaviour();
            clone.String = String;
            return playable;
        }
    }
}
