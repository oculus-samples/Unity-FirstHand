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

    public sealed class StandalonePlatform
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UnityLogDelegate(IntPtr tag, IntPtr msg);

        public Request<Models.PlatformInitialize> InitializeInEditor()
        {
#if UNITY_ANDROID
      if (String.IsNullOrEmpty(PlatformSettings.MobileAppID))
      {
        throw new UnityException("Update your App ID by selecting 'Oculus Platform' -> 'Edit Settings'");
      }
      var appID = PlatformSettings.MobileAppID;
#else
            if (String.IsNullOrEmpty(PlatformSettings.AppID))
            {
                throw new UnityException("Update your App ID by selecting 'Oculus Platform' -> 'Edit Settings'");
            }
            var appID = PlatformSettings.AppID;
#endif
            if (String.IsNullOrEmpty(StandalonePlatformSettings.OculusPlatformTestUserAccessToken))
            {
                throw new UnityException("Update your standalone credentials by selecting 'Oculus Platform' -> 'Edit Settings'");
            }
            var accessToken = StandalonePlatformSettings.OculusPlatformTestUserAccessToken;

            return AsyncInitialize(UInt64.Parse(appID), accessToken);
        }

        public Request<Models.PlatformInitialize> AsyncInitialize(ulong appID, string accessToken)
        {
            CAPI.ovr_UnityResetTestPlatform();
            CAPI.ovr_UnityInitGlobals(IntPtr.Zero);

            return new Request<Models.PlatformInitialize>(CAPI.ovr_PlatformInitializeWithAccessToken(appID, accessToken));
        }
    }
}
