// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Spawns instances and sets sibling index based on the count of an IList
    /// Can be used to control UI prefab instances where each instance represents an element in a list
    /// </summary>
    public class UIList : MonoBehaviour, ISerializationCallbackReceiver
    {
        private static readonly List<object> _toRemove = new List<object>();
        private static readonly List<IUIListElementHandler> _uiListElements = new List<IUIListElementHandler>();

        private IList _list;

        private Dictionary<object, GameObject> _instances = new Dictionary<object, GameObject>();

        /// <summary>
        /// Handles Creation and Destruction of instances
        /// </summary>
        public ILifeCycleHandler _lifeCycleHandler;

        [SerializeField, Tooltip("Instances will be spawned as a child of this transform and ordered as they appear in the list, " +
            "SiblingIndexOffset is used to offset the sibling order")]
        public int SiblingIndexOffset = 0;

        [SerializeField]
        private DefaultLifecycleHandler _defaultLifecycleHandler;

        public int InstanceCount => _instances.Count;

        /// <summary>
        /// Sets the current list this represents and Creates and Destroys instances to reflect its contents
        /// </summary>
        public void SetList(IList list, bool andUpdate = true)
        {
            _list = list;
            if (andUpdate) UpdateUI();
        }

        public void ForEach(System.Action<object, GameObject> action)
        {
            foreach (var pair in _instances) action(pair.Key, pair.Value);
        }

        /// <summary>
        /// Creates and Destroys instances to reflect the contents of the list
        /// </summary>
        public void UpdateUI()
        {
            var routine = UpdateUIRoutine();
            while (routine.MoveNext()) { };
        }

        /// <summary>
        /// Creates/destroys/reorders an individual element in the list
        /// </summary>
        public void UpdateUI(object element) => UpdateUI(element, _list.IndexOf(element));

        /// <summary>
        /// Creates/destroys/reorders an individual element in the list, a negative index indicates the element has been removed
        /// </summary>
        public void UpdateUI(object element, int index)
        {
            GameObject instance;
            if (index < 0 && _instances.TryGetValue(element, out instance))
            {
                _lifeCycleHandler.Destroy(instance);
                _instances.Remove(element);
            }
            else if (index >= 0)
            {
                if (!_instances.TryGetValue(element, out instance))
                {
                    _instances[element] = instance = _lifeCycleHandler.Create(element, transform);
                    InvokeHandleListElement(instance, element);
                }
                instance.transform.SetSiblingIndex(index + SiblingIndexOffset);
            }
        }

        public IEnumerator UpdateUIRoutine()
        {
            //in case the previous routine was cancelled we need to clear null entries
            if (_toRemove.Count > 0)
            {
                _toRemove.ForEach(x => _instances.Remove(x));
                _toRemove.Clear();
            }

            // destroy instances that are not in the list and mark the keys for removal from the dictionary
            foreach (var keyValue in _instances)
            {
                if (!_list.Contains(keyValue.Key))
                {
                    _toRemove.Add(keyValue.Key);
                    _lifeCycleHandler.Destroy(keyValue.Value);
                    yield return null;
                }
            }

            // remove old keys
            _toRemove.ForEach(x => _instances.Remove(x));
            _toRemove.Clear();

            GameObject instance;
            for (int i = 0; i < _list.Count; i++)
            {
                object element = _list[i];
                // create new instances for new elements if needed
                if (!_instances.TryGetValue(element, out instance))
                {
                    _instances[element] = instance = _lifeCycleHandler.Create(element, transform);
                    InvokeHandleListElement(instance, element);
                    yield return instance;
                }
                // order the siblings by thier order in the list
                instance.transform.SetSiblingIndex(i + SiblingIndexOffset);
            }
        }

        /// <summary>
        /// Invokes HandleListElement on components in the instance, passing the element as the argument, allowing them to populate themselves
        /// </summary>
        private static void InvokeHandleListElement(GameObject instance, object element)
        {
            instance.GetComponentsInChildren(_uiListElements);
            for (int j = 0; j < _uiListElements.Count; j++)
            {
#if UNITY_EDITOR
                try
                {
#endif
                _uiListElements[j].HandleListElement(element);
#if UNITY_EDITOR
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e, instance);
                }
#endif
            }
            _uiListElements.Clear();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_lifeCycleHandler == null) _lifeCycleHandler = _defaultLifecycleHandler;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        public interface ILifeCycleHandler
        {
            GameObject Create(object data, Transform parent);
            void Destroy(GameObject value);
        }

        /// <summary>
        /// Basic lifecycle handler that creates or destroys a prefab
        /// </summary>
        [System.Serializable]
        public struct DefaultLifecycleHandler : ILifeCycleHandler
        {
            [SerializeField]
            GameObject _prefab;
            [SerializeField]
            bool _usePool;

            private List<GameObject> _pool;
            private Transform _poolParent;

            public GameObject Create(object data, Transform parent)
            {
                // sometimes its inconvienent _not_ to have an actually prefab and just instantiate copies of a child
                if (_prefab.transform.parent == parent && _prefab.activeSelf)
                {
                    _prefab.SetActive(false);
                }

                if (_usePool && _poolParent == null)
                {
                    _poolParent = new GameObject($"{_prefab.name} Pool").transform;
                    _poolParent.gameObject.SetActive(false);
                    _poolParent.SetParent(parent);
                    _pool = new List<GameObject>();
                }

                GameObject gameObject = GetInstance(parent);
                gameObject.SetActive(true);

                return gameObject;
            }

            private readonly GameObject GetInstance(Transform parent)
            {
                if (_usePool && _pool.Count > 0)
                {
                    int last = _pool.Count - 1;
                    var result = _pool[last];
                    _pool.RemoveAt(last);
                    result.transform.SetParent(parent);
                    return result;
                }

                return Instantiate(_prefab, parent);
            }

            public void Destroy(GameObject value)
            {
                if (_usePool)
                {
                    value.transform.SetParent(_poolParent);
                    _pool.Add(value);
                }
                else
                {
                    Object.Destroy(value);
                }
            }
        }
    }

    /// <summary>
    /// When a prefab is instantiated for an element in a UIList, HandleListElement will be invoked on the instance
    /// </summary>
    public interface IUIListElementHandler
    {
        /// <summary>
        /// Called by UIList on a new instance when its created, used to allow spawned UI to populate itself
        /// </summary>
        /// <param name="element">The element of the list that caused this instance to be created</param>
        void HandleListElement(object element);
    }
}
