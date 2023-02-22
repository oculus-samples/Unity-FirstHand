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

  // This only exists for the Unity Editor
  public sealed class StandalonePlatformSettings
  {

#if UNITY_EDITOR
    private static string _OculusPlatformTestUserPassword = "";

    private static void ClearOldStoredPassword()
    {
      // Ensure that we are not storing the old passwords anywhere on the machine
      if (UnityEditor.EditorPrefs.HasKey("OculusStandaloneUserPassword"))
      {
        UnityEditor.EditorPrefs.SetString("OculusStandaloneUserPassword", "0000");
        UnityEditor.EditorPrefs.DeleteKey("OculusStandaloneUserPassword");
      }
    }
#endif

    public static string OculusPlatformTestUserEmail
    {
      get
      {
#if UNITY_EDITOR
        return UnityEditor.EditorPrefs.GetString("OculusStandaloneUserEmail");
#else
        return string.Empty;
#endif
      }
      set
      {
#if UNITY_EDITOR
        UnityEditor.EditorPrefs.SetString("OculusStandaloneUserEmail", value);
#endif
      }
    }

    public static string OculusPlatformTestUserPassword
    {
      get
      {
#if UNITY_EDITOR
        ClearOldStoredPassword();
        return _OculusPlatformTestUserPassword;
#else
        return string.Empty;
#endif
      }
      set
      {
#if UNITY_EDITOR
        ClearOldStoredPassword();
        _OculusPlatformTestUserPassword = value;
#endif
      }
    }
    public static string OculusPlatformTestUserAccessToken
    {
      get
      {
#if UNITY_EDITOR
        return UnityEditor.EditorPrefs.GetString("OculusStandaloneUserAccessToken");
#else
        return string.Empty;
#endif
      }
      set
      {
#if UNITY_EDITOR
        UnityEditor.EditorPrefs.SetString("OculusStandaloneUserAccessToken", value);
#endif
      }
    }
  }
}
