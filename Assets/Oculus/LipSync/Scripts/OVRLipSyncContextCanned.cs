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


[RequireComponent(typeof(AudioSource))]

//-------------------------------------------------------------------------------------
// ***** OVRLipSyncContextCanned
//
/// <summary>
/// OVRLipSyncContextCanned drives a canned phoneme sequence based on a pre-generated asset.
///
/// </summary>
public class OVRLipSyncContextCanned : OVRLipSyncContextBase
{
    [Tooltip("Pre-computed viseme sequence asset. Compute from audio in Unity with Tools -> Oculus -> Generate Lip Sync Assets.")]
    public OVRLipSyncSequence currentSequence;

    /// <summary>
    /// Run processes that need to be updated in game thread
    /// </summary>
    void Update()
    {
        if (audioSource.isPlaying && currentSequence != null)
        {
            OVRLipSync.Frame currentFrame = currentSequence.GetFrameAtTime(audioSource.time);
            this.Frame.CopyInput(currentFrame);
        }
    }
}
