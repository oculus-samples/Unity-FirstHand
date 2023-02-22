/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
