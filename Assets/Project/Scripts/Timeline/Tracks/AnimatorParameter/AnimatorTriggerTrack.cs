// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackClipType(typeof(AnimatorTriggerClip))]
    [TrackBindingType(typeof(Animator))]
    public class AnimatorTriggerTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = go.GetComponent<PlayableDirector>();
            var binding = director.GetGenericBinding(this);

            foreach (var c in GetClips())
            {
                var myAsset = c.asset as AnimatorTriggerClip;
                if (myAsset != null)
                    myAsset.binding = binding as Animator;
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }
    }

    [Serializable]
    public class AnimatorTriggerBehaviour : PlayableBehaviour
    {
        public string name;

        [NonSerialized]
        public Animator binding;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (Application.isPlaying)
            {
                binding.SetTrigger(name);
            }
        }
    }

    public class AnimatorTriggerMixerBehaviour : PlayableBehaviour
    {
    }
}
