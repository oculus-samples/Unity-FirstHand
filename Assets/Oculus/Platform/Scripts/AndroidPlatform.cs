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
    using System.Collections;
    using System;

    public class AndroidPlatform
    {
        public bool Initialize(string appId)
        {
#if UNITY_ANDROID
      if(String.IsNullOrEmpty(appId))
      {
        throw new UnityException("AppID must not be null or empty");
      }
      return CAPI.ovr_UnityInitWrapper(appId);
#else
            return false;
#endif
        }

        public Request<Models.PlatformInitialize> AsyncInitialize(string appId)
        {
#if UNITY_ANDROID
      if(String.IsNullOrEmpty(appId))
      {
        throw new UnityException("AppID must not be null or empty");
      }
      return new Request<Models.PlatformInitialize>(CAPI.ovr_UnityInitWrapperAsynchronous(appId));
#else
            return new Request<Models.PlatformInitialize>(0);
#endif
        }
    }
}
