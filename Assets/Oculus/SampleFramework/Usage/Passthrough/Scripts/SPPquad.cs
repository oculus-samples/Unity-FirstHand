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

public class SPPquad : MonoBehaviour
{
    OVRPassthroughLayer passthroughLayer;
    public MeshFilter projectionObject;
    OVRInput.Controller controllerHand;

    void Start()
    {
        passthroughLayer = GetComponent<OVRPassthroughLayer>();
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, false);
        if (GetComponent<GrabObject>())
        {
            GetComponent<GrabObject>().GrabbedObjectDelegate += Grab;
            GetComponent<GrabObject>().ReleasedObjectDelegate += Release;
        }
    }

    public void Grab(OVRInput.Controller grabHand)
    {
        passthroughLayer.RemoveSurfaceGeometry(projectionObject.gameObject);
        controllerHand = grabHand;
    }

    public void Release()
    {
        controllerHand = OVRInput.Controller.None;
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, false);
    }
}
