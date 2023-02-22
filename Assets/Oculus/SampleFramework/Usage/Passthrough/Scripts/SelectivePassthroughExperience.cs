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

public class SelectivePassthroughExperience : MonoBehaviour
{
    public GameObject leftMaskObject;
    public GameObject rightMaskObject;

    void Update()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;

        bool controllersActive = (OVRInput.GetActiveController() == OVRInput.Controller.LTouch ||
          OVRInput.GetActiveController() == OVRInput.Controller.RTouch ||
          OVRInput.GetActiveController() == OVRInput.Controller.Touch);

        leftMaskObject.SetActive(controllersActive);
        rightMaskObject.SetActive(controllersActive);

        // controller masks are giant circles attached to controllers
        if (controllersActive)
        {
            Vector3 Lpos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward * 0.1f;
            Vector3 Rpos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) + OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward * 0.1f;

            leftMaskObject.transform.position = Lpos;
            rightMaskObject.transform.position = Rpos;
        }
        // hand masks are an inflated hands shader, with alpha fading at wrists and edges
        else if (OVRInput.GetActiveController() == OVRInput.Controller.LHand ||
          OVRInput.GetActiveController() == OVRInput.Controller.RHand ||
          OVRInput.GetActiveController() == OVRInput.Controller.Hands)
        {

        }
    }
}
