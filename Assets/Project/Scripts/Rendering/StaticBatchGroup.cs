// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class StaticBatchGroup : MonoBehaviour
    {
        [SerializeField]
        private When _when = When.Build;

        [SerializeField]
        private List<GameObject> _exclude = new List<GameObject>();

        private void Start()
        {
            if (_when == When.Start) StaticBatch();
        }

        public void StaticBatch()
        {
            if (_exclude.Count == 0)
            {
                StaticBatchingUtility.Combine(gameObject);
            }
            else
            {
                StaticBatchingUtility.Combine(GetBatchableGameObjects(), gameObject);
            }

        }

        private GameObject[] GetBatchableGameObjects()
        {
            var renderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            var gameObjects = new List<GameObject>();
            foreach (var r in renderers)
            {
                if (ShouldExclude(r)) continue;
                gameObjects.Add(r.gameObject);
            }
            return gameObjects.ToArray();
        }

        private bool ShouldExclude(MeshRenderer r)
        {
            return _exclude.FindIndex(x => r.transform.IsChildOf(x.transform)) != -1;
        }

        public enum When
        {
            None,
            Build,
            Start
        }

#if UNITY_EDITOR
        class StaticBatchGroupProcessor : UnityEditor.Build.IProcessSceneWithReport
        {
            public int callbackOrder => 0;

            public void OnProcessScene(Scene scene, UnityEditor.Build.Reporting.BuildReport report)
            {
                var roots = scene.GetRootGameObjects();
                foreach (var root in roots)
                {
                    var batches = root.GetComponentsInChildren<StaticBatchGroup>(true);
                    foreach (var batch in batches)
                    {
                        if (batch._when == When.Build)
                        {
                            batch.StaticBatch();
                        }
                    }
                }
            }
        }
#endif
    }
}
