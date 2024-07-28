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

using System.Runtime.InteropServices;
using UnityEngine;

namespace Oculus.Platform
{
    public class CallbackRunner : MonoBehaviour
    {
        [DllImport(CAPI.DLL_NAME)]
        static extern void ovr_UnityResetTestPlatform();

        public bool IsPersistantBetweenSceneLoads = true;

        void Awake()
        {
            var existingCallbackRunner = FindObjectOfType<CallbackRunner>();
            if (existingCallbackRunner != this)
            {
                Debug.LogWarning("You only need one instance of CallbackRunner");
            }
            if (IsPersistantBetweenSceneLoads)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        void Update()
        {
            Request.RunCallbacks();
        }

        void OnDestroy()
        {
#if UNITY_EDITOR
      ovr_UnityResetTestPlatform();
#endif
        }

        void OnApplicationQuit()
        {
            Callback.OnApplicationQuit();
        }
    }
}
