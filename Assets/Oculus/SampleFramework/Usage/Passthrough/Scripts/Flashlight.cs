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

public class Flashlight : MonoBehaviour
{
    public GameObject lightVolume;
    public Light spotlight;
    public GameObject bulbGlow;

    void LateUpdate()
    {
        // ensure all the light volume quads are camera-facing
        for (int i = 0; i < lightVolume.transform.childCount; i++)
        {
            lightVolume.transform.GetChild(i).rotation = Quaternion.LookRotation((lightVolume.transform.GetChild(i).position - Camera.main.transform.position).normalized);
        }
    }

    public void ToggleFlashlight()
    {
        lightVolume.SetActive(!lightVolume.activeSelf);
        spotlight.enabled = !spotlight.enabled;
        bulbGlow.SetActive(lightVolume.activeSelf);
    }

    public void EnableFlashlight(bool doEnable)
    {
        lightVolume.SetActive(doEnable);
        spotlight.enabled = doEnable;
        bulbGlow.SetActive(doEnable);
    }
}
