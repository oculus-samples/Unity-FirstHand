// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls the loading of Levels as groups of scenes
    /// Handles caveats in SceneManagement e.g. having unactivated scenes prevents other scenes from unloading
    /// </summary>
    public class MasterLoader : MonoBehaviour
    {
        private static MasterLoader _instance;
        public static MasterLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MasterLoader>();
#if UNITY_EDITOR
                    if (!_instance)
                    {
                        // In the editor we can try to spawn the MasterLoader for faster testing when its missing
                        var assets = UnityEditor.AssetDatabase.FindAssets(nameof(MasterLoader)); //TODO not via name
                        for (int i = 0; i < assets.Length; i++)
                        {
                            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[i]);
                            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<MasterLoader>(path);
                            if (prefab != null)
                            {
                                // to facilitate unload the current scenes we need to add them to the _loadedLevels
                                _loadedLevels.Add(Level.FromLoadedScenes());
                                _instance = Instantiate(prefab);
                                return _instance;
                            }
                        }
                    }
#endif
                }
                return _instance;
            }
        }

        // getter that doesnt call Instantiate in editor which is slow
        static bool _triedFindObjects;
        public static bool ExistsInEditor
        {
            get
            {
                if (_instance == null && !_triedFindObjects)
                {
                    _triedFindObjects = true;
                    _instance = FindObjectOfType<MasterLoader>();
                }
                return _instance != null;
            }
        }

        static List<Level> _loadedLevels = new List<Level>();

        static Coroutine _tetrahedralizationProbesRoutine;

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            LightProbes.needsRetetrahedralization += MarkProbesDirty;
        }

        public static Level GetLevel(string name)
        {
            if (NoInstance(name)) return null;

            Level level = FindLevel(x => x._name == name);
            if (level == null) throw new Exception($"Couldn't find level {name}!");

            return level;
        }

        public static Level FindLevel(Predicate<Level> predicate)
        {
            if (NoInstance("FindLevel")) return null;

            return Instance._levels.Find(predicate);
        }

        public static async Task LoadLevel(string name)
        {
            if (NoInstance(name)) return;

            var level = GetLevel(name);
            await level.Load();
            _loadedLevels.Add(level);
        }

        private static bool NoInstance(string levelname)
        {
            bool noInstance = Instance == null;
            if (noInstance) Debug.LogError($"No MasterLoader, please play from the FirstLoad scene. Level:{levelname}");
            return noInstance;
        }

        public static async Task ActivateAndUnloadOthers(string name)
        {
            if (NoInstance(name)) return;

            var level = GetLevel(name);

            // We can't Unload scenes while other scenes are pending activation
            // OVRCameraRig breaks if 2 are loaded at once, we may have Players with OVRCameraRigs in more than one scene
            // Ideally we'd unload the old scene then activate the pending scene, but Unity can't.
            // A compromise is to 'Deactivate' the scene to unload (Destroy everything in it)
            // Then activate the pending scene, then unload the old scene

            // We can't Unload a scene that's loading or pending activation
            // We need to Activate the scene then Deactivate it then Unload it

            for (int i = _loadedLevels.Count - 1; i >= 0; i--)
            {
                Level loadedLevel = _loadedLevels[i];
                if (loadedLevel == level) continue;

                if (loadedLevel.state == Level.State.Loading) await loadedLevel.CurrentTask();
                if (loadedLevel.state == Level.State.Loaded) await loadedLevel.Activate();
                if (loadedLevel.state == Level.State.Activating) await loadedLevel.CurrentTask();
                if (loadedLevel.state == Level.State.Active) await loadedLevel.Deactivate();
            }

            await level.Activate();

            for (int i = _loadedLevels.Count - 1; i >= 0; i--)
            {
                if (_loadedLevels[i] == level) continue;

                await _loadedLevels[i].Unload();
                _loadedLevels.RemoveAt(i);
            }

            var firstLoad = SceneManager.GetSceneByName("FirstLoad");
            if (firstLoad.IsValid() && firstLoad.isLoaded)
            {
                SceneManager.UnloadSceneAsync(firstLoad);
            }
        }

        private static void MarkProbesDirty()
        {
            if (_tetrahedralizationProbesRoutine != null) return;

            _tetrahedralizationProbesRoutine = Instance.StartCoroutine(UpdateProbes());
            IEnumerator UpdateProbes()
            {
                yield return new WaitWhile(() => _loadedLevels.TrueForAny(x => x.state == Level.State.Activating));
                yield return null;

                bool complete = false;
                Action setComplete = () => complete = true;

                LightProbes.tetrahedralizationCompleted += setComplete;
                LightProbes.TetrahedralizeAsync();

                yield return new WaitUntil(() => complete);

                LightProbes.tetrahedralizationCompleted -= setComplete;

                _tetrahedralizationProbesRoutine = null;
            }
        }

        public static async Task UnloadAll()
        {
            if (NoInstance("UnloadAll")) return;

            for (int i = _loadedLevels.Count - 1; i >= 0; i--)
            {
                await _loadedLevels[i].Deactivate();
                await _loadedLevels[i].Unload();
                _loadedLevels.RemoveAt(i);
            }
        }

        [SerializeField]
        List<Level> _levels;
        private static bool _isLightProbesDirty;

        /// <summary>
        /// A wrapper for a set of Scenes that should be treated as one unit
        /// A Level's loading process goes None > Loading > Loaded (meaning it's loaded to 0.9, not yet activated) > Activating > Active (meaning done and playing)
        /// A Level's unloading process goes Active > Deactivated (meaning all it's content has been destroyed) > Unloading > None
        /// </summary>
        [Serializable]
        public class Level
        {
            public readonly static State StateNone = 0;

            [SerializeField]
            public string _name;

            [SerializeField]
            int _activeIndex = -1;

            [SerializeField]
            List<string> _scenes;

            public State state { get; private set; }
            public bool IsActiveOrActivating => state == State.Activating || state == State.Active;

            private List<AsyncOperation> _ops = new List<AsyncOperation>();
            private TaskCompletionSource<bool> _loadingTask;
            private TaskCompletionSource<bool> _activatingTask;
            private TaskCompletionSource<bool> _unloadingTask;

            public Task Load()
            {
                if (state != StateNone) throw new Exception($"Cant Load {_name} while it's {state}");

                state = State.Loading;

                for (int i = 0; i < _scenes.Count; i++)
                {
                    var op = SceneManager.LoadSceneAsync(_scenes[i], LoadSceneMode.Additive);
                    op.allowSceneActivation = false;
                    _ops.Add(op);
                }

                _loadingTask = new TaskCompletionSource<bool>();
                CoroutineRunner.Run(LoadRoutine());
                IEnumerator LoadRoutine()
                {
                    yield return null;
                    while (!_ops.TrueForAll(x => x.progress >= 0.9f))
                    {
                        yield return null;
                    }
                    state = State.Loaded;
                    _loadingTask.SetResult(true);
                    _loadingTask = null;
                }
                return _loadingTask.Task;
            }

            public Task Activate()
            {
                if (state != State.Loaded) throw new Exception($"Cant Activate {_name} while it's {state}");

                state = State.Activating;

                for (int i = 0; i < _ops.Count; i++)
                {
                    _ops[i].allowSceneActivation = true;
                }

                _activatingTask = new TaskCompletionSource<bool>();
                CoroutineRunner.Run(ActivateRoutine());
                IEnumerator ActivateRoutine()
                {
                    yield return null;
                    while (!_ops.TrueForAll(x => x.isDone))
                    {
                        yield return null;
                    }

                    if (_activeIndex >= 0)
                    {
                        Scene scene = SceneManager.GetSceneByName(_scenes[_activeIndex]);
                        var success = SceneManager.SetActiveScene(scene);
                        if (!success)
                        {
                            Debug.LogError($"Could not set {scene.name} as the Active scene!");
                        }
                    }

                    state = State.Active;
                    _ops.Clear();
                    _activatingTask.SetResult(true);
                    _activatingTask = null;
                }
                return _activatingTask.Task;
            }

            public Task Deactivate()
            {
                if (state != State.Active) throw new Exception($"Cant Deactivate {_name} while it's {state}");

                for (int i = _scenes.Count - 1; i >= 0; i--)
                {
                    var scene = SceneManager.GetSceneByName(_scenes[i]);
                    var roots = scene.GetRootGameObjects();
                    foreach (var go in roots)
                    {
                        Destroy(go);
                    }
                }

                state = State.Deactivated;
                return Task.CompletedTask;
            }

            public Task Unload()
            {
                if (!(state == State.Deactivated || state == State.Loaded)) throw new Exception($"Can't Unload {_name} while it's {state}");

                state = State.Unloading;

                for (int i = 0; i < _scenes.Count; i++)
                {
                    var op = SceneManager.UnloadSceneAsync(_scenes[i]);
                    _ops.Add(op);
                }

                _unloadingTask = new TaskCompletionSource<bool>();
                CoroutineRunner.Run(UnloadRoutine());
                IEnumerator UnloadRoutine()
                {
                    yield return null;
                    while (!_ops.TrueForAll(x => x.isDone))
                    {
                        yield return null;
                    }
                    state = StateNone;
                    _ops.Clear();
                    _unloadingTask.SetResult(true);
                    _unloadingTask = null;
                }
                return _unloadingTask.Task;
            }

            public Task CurrentTask()
            {
                switch (state)
                {
                    case State.Loading:
                        return _loadingTask.Task;
                    case State.Activating:
                        return _activatingTask.Task;
                    case State.Unloading:
                        return _unloadingTask.Task;
                    default:
                        return null;
                }
            }

            [Flags]
            public enum State
            {
                Loading = 1 << 0,
                Loaded = 1 << 1,
                Activating = 1 << 2,
                Active = 1 << 3,
                Deactivated = 1 << 4,
                Unloading = 1 << 5
            }

            /// <summary>
            /// Creates a Level that contains the current loaded scenes</br>
            /// Used to facilitate Loading in the editor not from first load
            /// </summary>
            public static Level FromLoadedScenes()
            {
                var result = new Level();
                result.state = State.Loaded;
                result._name = "loaded scenes";
                result._scenes = new List<string>();
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    result._scenes.Add(SceneManager.GetSceneAt(i).name);
                }
                return result;
            }
        }

        /// <summary>
        /// Singleton for convinience running coroutines
        /// </summary>
        public class CoroutineRunner : MonoBehaviour
        {
            static CoroutineRunner _instance;

            static void Exist()
            {
                if (_instance || !Application.isPlaying) { return; }

                _instance = new GameObject(nameof(CoroutineRunner)).AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            public static Coroutine Run(IEnumerator routine)
            {
                Exist();
                return _instance.StartCoroutine(routine);
            }
        }
    }

    public static class ListExtensions
    {
        public static bool TrueForAny<T>(this List<T> list, Predicate<T> p)
        {
            return !list.TrueForAll(x => !p(x));
        }
    }
}
