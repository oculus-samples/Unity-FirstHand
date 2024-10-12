// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Removes 'EditorOnly' tagged gameobjects from the scene in PlayMode, to better simulate how it will appear in the build
    /// </summary>
    public class RemoveEditorOnlyProcessScene : IProcessSceneWithReport
    {
        public int callbackOrder => -100;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            var roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                var children = roots[i].GetComponentsInChildren<Transform>(true);
                for (int j = children.Length - 1; j >= 0; j--)
                {
                    if (children[j] && children[j].CompareTag("EditorOnly"))
                    {
                        Object.DestroyImmediate(children[j].gameObject);
                    }
                }
            }
        }
    }
}
