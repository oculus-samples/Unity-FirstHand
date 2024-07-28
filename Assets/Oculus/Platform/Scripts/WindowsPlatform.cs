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

namespace Oculus.Platform
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class WindowsPlatform
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UnityLogDelegate(IntPtr tag, IntPtr msg);

        void CPPLogCallback(IntPtr tag, IntPtr message)
        {
            Debug.Log(string.Format("{0}: {1}", Marshal.PtrToStringAnsi(tag), Marshal.PtrToStringAnsi(message)));
        }

        IntPtr getCallbackPointer()
        {
            //UnityLogDelegate callback_delegate = new UnityLogDelegate(CPPLogCallback);
            //IntPtr intptr_delegate = Marshal.GetFunctionPointerForDelegate(callback_delegate);
            return IntPtr.Zero;
        }

        public bool Initialize(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                throw new UnityException("AppID must not be null or empty");
            }

            CAPI.ovr_UnityInitWrapperWindows(appId, getCallbackPointer());
            return true;
        }

        public Request<Models.PlatformInitialize> AsyncInitialize(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                throw new UnityException("AppID must not be null or empty");
            }

            return new Request<Models.PlatformInitialize>(CAPI.ovr_UnityInitWrapperWindowsAsynchronous(appId, getCallbackPointer()));
        }
    }
}
