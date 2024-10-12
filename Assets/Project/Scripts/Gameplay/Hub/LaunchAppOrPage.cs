// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Launches a web page / app
    /// </summary>
    public class LaunchAppOrPage : MonoBehaviour
    {
        [SerializeField]
        string _appID = "";

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OpenStorePage);
        }

        private void OpenStorePage() => OpenOculusStorePage(_appID);

        public static void OpenOculusStorePage(string appID)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    //https://communityforums.atmeta.com/t5/Quest-Development/Linking-To-An-App-s-Store-Page-Review-Tab/m-p/841950/highlight/true#M2704
                    using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    using (var packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager"))
                    using (var i = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", "com.oculus.vrshell"))
                    {
                        i.Call<AndroidJavaObject>("setClassName", "com.oculus.vrshell", "com.oculus.vrshell.MainActivity");
                        i.Call<AndroidJavaObject>("setAction", "android.intent.action.VIEW");
                        if (!string.IsNullOrWhiteSpace(appID))
                        {
                            i.Call<AndroidJavaObject>("putExtra", "uri", "/item/" + appID);
                        }
                        i.Call<AndroidJavaObject>("putExtra", "intent_data", "systemux://store");
                        currentActivity.Call("startActivity", i);
                    }
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            Application.OpenURL($"https://www.oculus.com/experiences/quest/{appID}");
        }
    }
}
