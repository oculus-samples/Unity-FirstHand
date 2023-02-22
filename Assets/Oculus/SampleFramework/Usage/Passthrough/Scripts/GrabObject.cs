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

public class GrabObject : MonoBehaviour
{
    [TextArea]
    public string ObjectName;
    [TextArea]
    public string ObjectInstructions;
    public enum ManipulationType
    {
        Default,
        ForcedHand,
        DollyHand,
        DollyAttached,
        HorizontalScaled,
        VerticalScaled,
        Menu
    };
    public ManipulationType objectManipulationType = ManipulationType.Default;
    public bool showLaserWhileGrabbed = false;
    [HideInInspector]
    public Quaternion grabbedRotation = Quaternion.identity;

    // only handle grab/release
    // other button input is handled by another script on the object
    public delegate void GrabbedObject(OVRInput.Controller grabHand);
    public GrabbedObject GrabbedObjectDelegate;

    public delegate void ReleasedObject();
    public ReleasedObject ReleasedObjectDelegate;

    public delegate void SetCursorPosition(Vector3 cursorPosition);
    public SetCursorPosition CursorPositionDelegate;

    public void Grab(OVRInput.Controller grabHand)
    {
        grabbedRotation = transform.rotation;
        GrabbedObjectDelegate?.Invoke(grabHand);
    }

    public void Release()
    {
        ReleasedObjectDelegate?.Invoke();
    }

    public void CursorPos(Vector3 cursorPos)
    {
        CursorPositionDelegate?.Invoke(cursorPos);
    }
}
