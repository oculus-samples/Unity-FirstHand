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
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{

    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(ImageFillClip))]
    [TrackBindingType(typeof(Image))]
    public class ImageFillTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ImageFillMixerBehaviour>.Create(graph, inputCount);
        }

        // Please note this assumes only one component of type Image on the same gameobject.
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            Image trackBinding = director.GetGenericBinding(this) as Image;
            if (trackBinding == null)
            {
                return;
            }
            driver.AddFromName<Image>(trackBinding.gameObject, "m_FillAmount");
#endif
            base.GatherProperties(director, driver);
        }
    }
}
