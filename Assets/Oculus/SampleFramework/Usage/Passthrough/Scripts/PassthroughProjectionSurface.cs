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

public class PassthroughProjectionSurface : MonoBehaviour
{
    private OVRPassthroughLayer passthroughLayer;
    public MeshFilter projectionObject;
    MeshRenderer quadOutline;

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

        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, true);

        // The MeshRenderer component renders the quad as a blue outline
        // we only use this when Passthrough isn't visible
        quadOutline = projectionObject.GetComponent<MeshRenderer>();
        quadOutline.enabled = false;
    }

    void Update()
    {
        // Hide object when A button is held, show it again when button is released, move it while held.
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            passthroughLayer.RemoveSurfaceGeometry(projectionObject.gameObject);
            quadOutline.enabled = true;
        }
        if (OVRInput.Get(OVRInput.Button.One))
        {
            OVRInput.Controller controllingHand = OVRInput.Controller.RTouch;
            transform.position = OVRInput.GetLocalControllerPosition(controllingHand);
            transform.rotation = OVRInput.GetLocalControllerRotation(controllingHand);
        }
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject);
            quadOutline.enabled = false;
        }
    }
}
