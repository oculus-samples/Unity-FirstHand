/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class ImageFillClip : PlayableAsset, ITimelineClipAsset
    {
        public ImageFillBehaviour Template = new ImageFillBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending | ClipCaps.Extrapolation; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<ImageFillBehaviour>.Create(graph, Template);
        }
    }
}
