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

public class EnableSwitch : MonoBehaviour
{
    public GameObject[] SwitchTargets;

    /// <summary>
    /// Sets the active GameObject
    /// </summary>
    /// <returns><c>true</c>, if active was set, <c>false</c> otherwise.</returns>
    /// <param name="target">Target.</param>
    public bool SetActive<T>(int target) where T : MonoBehaviour
    {
        if((target < 0) || (target >= SwitchTargets.Length))
            return false;

        for (int i = 0; i < SwitchTargets.Length; i++)
        {
            SwitchTargets[i].SetActive(false);

            // Disable texture flip or morph target
            OVRLipSyncContextMorphTarget lipsyncContextMorph =
                   SwitchTargets[i].GetComponent<OVRLipSyncContextMorphTarget>();
            if (lipsyncContextMorph)
                lipsyncContextMorph.enabled = false;
            OVRLipSyncContextTextureFlip lipsyncContextTexture =
                   SwitchTargets[i].GetComponent<OVRLipSyncContextTextureFlip>();
            if (lipsyncContextTexture)
                lipsyncContextTexture.enabled = false;
        }

        SwitchTargets[target].SetActive(true);
        MonoBehaviour lipsyncContext = SwitchTargets[target].GetComponent<T>();
        if (lipsyncContext != null)
        {
            lipsyncContext.enabled = true;
        }

        return true;
    }
}

