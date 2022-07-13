/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.4185208f, 0.6573736f, 0.8962264f)]
    [TrackClipType(typeof(CanvasGroupClip))]
    [TrackBindingType(typeof(CanvasGroup))]
    public class CanvasGroupTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<CanvasGroupMixerBehaviour>.Create(graph, inputCount);
        }

        // Please note this assumes only one component of type CanvasGroup on the same gameobject.
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            CanvasGroup trackBinding = director.GetGenericBinding(this) as CanvasGroup;
            if (trackBinding == null)
            {
                return;
            }
            driver.AddFromName<CanvasGroup>(trackBinding.gameObject, "m_Alpha");
#endif
            base.GatherProperties(director, driver);
        }
    }
}
