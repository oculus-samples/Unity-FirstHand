// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
#if UNITY_EDITOR
using System.ComponentModel;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;



namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
#if UNITY_EDITOR
    [DisplayName("Active")]
#endif
    public class ConfigActiveStateClip : PlayableAsset, ITimelineClipAsset
    {
        private ConfigActiveStateBehaviour _template = new ConfigActiveStateBehaviour();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<ConfigActiveStateBehaviour>.Create(graph, _template);
        }
    }
}
