// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Takes a StringProperty and PlayerPref entry in sync
    /// </summary>
    public class StringPropertyPlayerPref : MonoBehaviour
    {
        [SerializeField]
        private StringPropertyRef _stringProperty;
        [SerializeField]
        private string _playerPrefKey;
        [SerializeField]
        private bool _clearInEditor = true;
        [SerializeField]
        private bool _clearInBuild = false;
        [SerializeField]
        private bool _emptyByDefault;
        [SerializeField]
        private ReferenceActiveState _permanent = ReferenceActiveState.Optional();

        private bool _isPermanent;
        private bool _initialized;

        internal StringPropertyRef Property => _stringProperty;
        public string Key => _playerPrefKey;

        private void Reset()
        {
            _stringProperty.Property = GetComponent<IProperty<string>>();
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            _isPermanent = _permanent;

            if (_clearInEditor && Application.isEditor) { Store.DeleteKey(_playerPrefKey, _isPermanent); }
            if (_clearInBuild && !Application.isEditor) { Store.DeleteKey(_playerPrefKey, _isPermanent); }

            _stringProperty.AssertNotNull();

            bool hasKey = Store.HasKey(_playerPrefKey);
            if (hasKey || _emptyByDefault)
            {
                string value = hasKey ? Store.GetString(_playerPrefKey) : "";
                try
                {
                    _stringProperty.Value = value;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            _stringProperty.WhenChanged += UpdatePlayerPref;
            Store.WhenChanged += UpdateStringProperty;
        }

        private void Update()
        {
            bool permanent = _permanent;
            if (permanent == _isPermanent) return;

            _isPermanent = permanent;
            if (_isPermanent)
            {
                Store.SetString(_playerPrefKey, _stringProperty.Value, false);
                Store.MakePermanent(_playerPrefKey);
            }
        }

        private void OnDestroy()
        {
            _stringProperty.WhenChanged -= UpdatePlayerPref;
            Store.WhenChanged -= UpdateStringProperty;

            if (!_isPermanent) Store.ClearTemporary(_playerPrefKey);
        }

        private void UpdateStringProperty()
        {
            bool hasKey = Store.HasKey(_playerPrefKey);
            if (hasKey || _emptyByDefault)
            {
                _stringProperty.WhenChanged -= UpdatePlayerPref;
                _stringProperty.Value = hasKey ? Store.GetString(_playerPrefKey) : "";
                _stringProperty.WhenChanged += UpdatePlayerPref;
            }
        }

        private void UpdatePlayerPref()
        {
            Store.WhenChanged -= UpdateStringProperty;
            Store.SetString(_playerPrefKey, _stringProperty.Value, _isPermanent);
            Store.WhenChanged += UpdateStringProperty;
        }

        [ContextMenu("Clear")]
        private void Clear()
        {
            Store.DeleteKey(_playerPrefKey);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StringPropertyPlayerPref), true), CanEditMultipleObjects]
    class StringPropertyPlayerPrefEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var key = serializedObject.FindProperty("_playerPrefKey");
            if (key.hasMultipleDifferentValues)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Value", "-");
                EditorGUI.EndDisabledGroup();
            }
            else if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Value", Store.GetString(key.stringValue));
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                GUI.color = new Color(1, 1, 1, 0.5f);
                var value = Store.GetString(key.stringValue);
                var newValue = EditorGUILayout.DelayedTextField("Value", value);
                if (newValue != value)
                {
                    Store.SetString(key.stringValue, newValue);
                }
            }
        }
    }
#endif
}
