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

using UnityEditor;

public class FirebaseEnabler
{
    private static readonly string FirebaseBuildDefinition = "OVR_SAMPLES_ENABLE_FIREBASE";

    [MenuItem("Oculus/Samples/Firebase/Enable Firebase Sample")]
    public static void EnableFirebaseSample()
    {
        var defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, $"{defineString};{FirebaseBuildDefinition}");
    }

    [MenuItem("Oculus/Samples/Firebase/Disable Firebase Sample")]
    public static void DisableFirebaseSample()
    {
        var defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        var defines = defineString.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
        var filtered = System.Array.FindAll(defines, d => d != FirebaseBuildDefinition);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, string.Join(";", filtered));
    }
}
