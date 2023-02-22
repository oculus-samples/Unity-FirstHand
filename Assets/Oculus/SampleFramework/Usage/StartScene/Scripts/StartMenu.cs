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
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Create menu of all scenes included in the build.
public class StartMenu : MonoBehaviour
{
    public OVROverlay overlay;
    public OVROverlay text;
    public OVRCameraRig vrRig;

    void Start()
    {
        DebugUIBuilder.instance.AddLabel("Select Sample Scene");

        int n = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < n; ++i)
        {
            string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            var sceneIndex = i;
            DebugUIBuilder.instance.AddButton(Path.GetFileNameWithoutExtension(path), () => LoadScene(sceneIndex));
        }

        DebugUIBuilder.instance.Show();
    }

    void LoadScene(int idx)
    {
        DebugUIBuilder.instance.Hide();
        Debug.Log("Load scene: " + idx);
        UnityEngine.SceneManagement.SceneManager.LoadScene(idx);
    }
}
