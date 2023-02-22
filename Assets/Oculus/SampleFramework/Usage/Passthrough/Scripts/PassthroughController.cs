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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughController : MonoBehaviour
{
    OVRPassthroughLayer passthroughLayer;

    void Start()
    {
        GameObject ovrCameraRig = GameObject.Find("OVRCameraRig");
        if (ovrCameraRig == null)
        {
            Debug.LogError("Scene does not contain an OVRCameraRig");
            return;
        }

        passthroughLayer = ovrCameraRig.GetComponent<OVRPassthroughLayer>();
        if (passthroughLayer == null)
        {
            Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
        }
    }

    void Update()
    {
        float colorHSV = Time.time * 0.1f;
        colorHSV %= 1; // make sure value is normalized (0-1) so Color.HSVToRGB functions correctly
        Color edgeColor = Color.HSVToRGB(colorHSV, 1, 1);
        passthroughLayer.edgeColor = edgeColor;

        float contrastRange = Mathf.Sin(Time.time); // returns a value -1...1, ideal range for contrast
        passthroughLayer.SetColorMapControls(contrastRange);

        transform.position = Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z).normalized);
    }
}
