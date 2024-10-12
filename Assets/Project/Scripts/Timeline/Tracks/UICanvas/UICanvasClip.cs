// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class UICanvasClip : PlayableAsset, ITimelineClipAsset
    {
        public UICanvasBehaviour template = new UICanvasBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending | ClipCaps.Extrapolation; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<UICanvasBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
