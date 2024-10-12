// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class AudioSettingsBuildProcess : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => -1;

        public void OnPostprocessBuild(BuildReport report)
        {
            SetDSPBufferSize(1024);
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            var platform = EditorUserBuildSettings.activeBuildTarget;
            var buffer = platform == BuildTarget.Android ? 256 : 1024;
            SetDSPBufferSize(buffer);
        }

        private static void SetDSPBufferSize(int V)
        {
            var audioManager = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/AudioManager.asset");

            var serObj = new SerializedObject(audioManager);
            serObj.Update();

            var spatializerProperty = serObj.FindProperty("m_DSPBufferSize");
            spatializerProperty.intValue = V;

            serObj.ApplyModifiedProperties();
        }
    }
}
