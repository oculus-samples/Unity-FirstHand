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

namespace Oculus.Platform.Samples.VrHoops
{
    using UnityEngine;

    // Helper class to attach to the MainCamera so it can be moved with the mouse while debugging
    // in 2D mode on a PC.
    public class Camera2DController : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetButton("Fire2"))
            {
                var v = Input.GetAxis("Mouse Y");
                var h = Input.GetAxis("Mouse X");
                transform.rotation *= Quaternion.AngleAxis(h, Vector3.up);
                transform.rotation *= Quaternion.AngleAxis(-v, Vector3.right);
                Vector3 eulers = transform.eulerAngles;
                eulers.z = 0;
                transform.eulerAngles = eulers;
            }
        }
    }
}
