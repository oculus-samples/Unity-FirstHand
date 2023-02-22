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
using UnityEngine.SceneManagement;

public class SceneSampler : MonoBehaviour
{
    int currentSceneIndex = 0;
    public GameObject displayText;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        bool controllersActive = OVRInput.GetActiveController() == OVRInput.Controller.Touch ||
          OVRInput.GetActiveController() == OVRInput.Controller.LTouch ||
          OVRInput.GetActiveController() == OVRInput.Controller.RTouch;

        displayText.SetActive(controllersActive);

        if (OVRInput.GetUp(OVRInput.Button.Start))
        {
            currentSceneIndex++;
            if (currentSceneIndex >= SceneManager.sceneCountInBuildSettings) currentSceneIndex = 0;
            SceneManager.LoadScene(currentSceneIndex);
        }

        Vector3 menuPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + Vector3.up * 0.09f;
        displayText.transform.position = menuPosition;
        displayText.transform.rotation = Quaternion.LookRotation(menuPosition - Camera.main.transform.position);
    }
}
