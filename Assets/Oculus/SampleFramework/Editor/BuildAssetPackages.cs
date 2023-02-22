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

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BuildAssetPackages
{
    enum BuildConfiguration
    {
        Windows,
        Android
    }

    public static void Build()
    {
        Debug.Log("Building Deliverables");
        ExportPackages();
    }

    public static void ExportPackages()
    {
        string[] assets = AssetDatabase.FindAssets("t:Object", null).Select(s=>AssetDatabase.GUIDToAssetPath(s)).ToArray();
        assets = assets.Where(s=>
            s.StartsWith("Assets/Oculus/AudioManager/") ||
            s.StartsWith("Assets/Oculus/LipSync/") ||
            s.StartsWith("Assets/Oculus/Platform/") ||
            s.StartsWith("Assets/Oculus/Spatializer/") ||
            s.StartsWith("Assets/Oculus/Voice/") ||
            s.StartsWith("Assets/Oculus/Interaction/") ||
            s.StartsWith("Assets/Oculus/VoiceMod/") ||
            s.StartsWith("Assets/Oculus/VR/") ||
            s.StartsWith("Assets/Oculus/SampleFramework/")
        ).ToArray();
        AssetDatabase.ExportPackage(assets, "OculusIntegration.unitypackage");
    }
}
