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

public class AugmentedObject : MonoBehaviour
{
    public OVRInput.Controller controllerHand = OVRInput.Controller.None;
    public Transform shadow;
    bool groundShadow = false;

    void Start()
    {
        if (GetComponent<GrabObject>())
        {
            GetComponent<GrabObject>().GrabbedObjectDelegate += Grab;
            GetComponent<GrabObject>().ReleasedObjectDelegate += Release;
        }
    }

    void Update()
    {
        if (controllerHand != OVRInput.Controller.None)
        {
            if (OVRInput.GetUp(OVRInput.Button.One, controllerHand))
            {
                ToggleShadowType();
            }
        }

        if (shadow)
        {
            if (groundShadow)
            {
                shadow.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            else
            {
                shadow.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void Grab(OVRInput.Controller grabHand)
    {
        controllerHand = grabHand;
    }

    public void Release()
    {
        controllerHand = OVRInput.Controller.None;
    }

    void ToggleShadowType()
    {
        groundShadow = !groundShadow;
    }
}
