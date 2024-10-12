// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Oculus.Interaction.ComprehensiveSample.HubManager;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// A wrapper for PlayerPrefs, that has a callback when a value is changed
    /// </summary>
    public static class Store
    {
        /// <summary>
        /// Keys that will survive DeleteAll
        /// </summary>
        public static readonly List<string> ImportantKeys = new List<string>()
        {
            _versionKey
        };

        public static event Action WhenChanged;

        private const string _shadowPrefsKey = "game";
        private const string _versionKey = "version";

        private static Dictionary<string, string> _temporaryPrefs = new Dictionary<string, string>();
        private static HashSet<string> _temporaryDeletedKeys = new HashSet<string>();

        private static Dictionary<string, string> _shadowPrefs = new Dictionary<string, string>();

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            LoadShadowPrefs();

            string previousVersion = HasKey(_versionKey) ? GetString(_versionKey) : string.Empty;

            if (string.IsNullOrEmpty(previousVersion) && PlayerPrefs.HasKey(_versionKey))
            {
                previousVersion = PlayerPrefs.GetString(_versionKey, string.Empty);
                PlayerPrefs.DeleteKey(_versionKey);
            }

            string currentVersion = Application.version;

            if (previousVersion != currentVersion)
            {
                UpdateSavedData();

                SetString(_versionKey, currentVersion);
                Debug.Log($"Version updated from {previousVersion} to {currentVersion}");
            }

            Application.focusChanged += _ => SaveShadowPrefs();
            Application.quitting += SaveShadowPrefs;


            void UpdateSavedData()
            {
                UpdateKey("settings.seated_mode");

                if (UpdateKey("glove.colors.fingers") | //dont short circuit
                    UpdateKey("glove.colors.palm") |
                    UpdateKey("glove.colors.crystal") |
                    UpdateKey("glove.colors.knuckles") |
                    UpdateKey("glove.colors.thumb"))
                {
                    SetString("clocktower.completed", "yes");
                    SetString("hub.state", JsonUtility.ToJson(new HubState { clocktowerIntroPlayed = true }));
                }

                // updates a key from the old system to the new system
                bool UpdateKey(string key)
                {
                    if (!PlayerPrefs.HasKey(key)) return false;
                    SetString(key, PlayerPrefs.GetString(key));
                    PlayerPrefs.DeleteKey(key);
                    Debug.Log($"Updated {key}");
                    return true;
                }
            }
        }


        public static int GetInt(string key)
        {
            if (!HasKey(key)) return 0;

            var str = GetString(key);
            int.TryParse(str, out int result);
            return result;
        }

        public static void SetInt(string key, int value) => SetString(key, value.ToString());

        public static float GetFloat(string key)
        {
            if (!HasKey(key)) return 0;

            var str = GetString(key);
            float.TryParse(str, out float result);
            return result;
        }

        public static void DeleteAll(bool deleteImportant = false)
        {
            var keysToDelete = new List<string>(_shadowPrefs.Keys);
            if (!deleteImportant) keysToDelete.RemoveAll(x => ImportantKeys.Contains(x));

            for (int i = 0; i < keysToDelete.Count; i++)
            {
                var key = keysToDelete[i];
                _shadowPrefs.Remove(key);
                _temporaryPrefs.Remove(key);
                _temporaryDeletedKeys.Remove(key);
            }

            SaveShadowPrefs();
            WhenChanged?.Invoke();
        }

        public static void SetFloat(string key, float value) => SetString(key, value.ToString("N1"));

        public static void SetMaxFloat(string key, float value) => SetFloat(key, Mathf.Max(value, GetFloat(key)));

        public static void SetMaxInt(string key, int value) => SetInt(key, Math.Max(value, GetInt(key)));

        public static void Increment(string key) => SetInt(key, GetInt(key) + 1);

        public static bool HasKey(string key, bool includeTemporary = true)
        {
            if (includeTemporary && _temporaryPrefs.ContainsKey(key)) return true;
            if (includeTemporary && _temporaryDeletedKeys.Contains(key)) return false;
            return _shadowPrefs.ContainsKey(key);
        }

        public static void SetString(string key, string value, bool permanent = true)
        {
            if (permanent)
            {
                _shadowPrefs[key] = value;
                if (!Application.isPlaying) SaveShadowPrefs();
            }
            else
            {
                _temporaryPrefs[key] = value;
            }

            WhenChanged?.Invoke();
        }

        public static string GetString(string key)
        {
            if (_temporaryPrefs.ContainsKey(key))
            {
                return _temporaryPrefs[key];
            }

            if (_temporaryDeletedKeys.Contains(key))
            {
                return string.Empty;
            }

            return _shadowPrefs.TryGetValue(key, out var value) ? value : "";
        }

        public static bool MakePermanent(string key)
        {
            if (!_temporaryPrefs.ContainsKey(key)) return false;

            var value = _temporaryPrefs[key];
            _temporaryPrefs.Remove(key);
            _temporaryDeletedKeys.Remove(key);

            _shadowPrefs[key] = value;
            if (!Application.isPlaying) SaveShadowPrefs();

            return true;
        }

        public static void DeleteKey(string key, bool permanent = true, bool invokeChange = true)
        {
            if (permanent)
            {
                _shadowPrefs.Remove(key);
            }
            else
            {
                _temporaryPrefs.Remove(key);
                _temporaryDeletedKeys.Add(key);
            }

            if (invokeChange)
            {
                WhenChanged?.Invoke();
            }
        }

        public static bool ClearTemporary(string key)
        {
            _temporaryDeletedKeys.Remove(key);
            return _temporaryPrefs.Remove(key);
        }

        public static void GetAllKeys(List<string> results, Predicate<string> keyPredicate = null)
        {
            results.Clear();

            foreach (var keyValue in _temporaryPrefs)
            {
                if (keyPredicate != null && !keyPredicate(keyValue.Key)) continue;
                results.Add(keyValue.Key);
            }

            foreach (var keyValue in _shadowPrefs)
            {
                var key = keyValue.Key;
                if (_temporaryPrefs.ContainsKey(key)) continue;
                if (_temporaryDeletedKeys.Contains(key)) continue;
                if (keyPredicate != null && !keyPredicate(key)) continue;
                results.Add(key);
            }
        }

        public static void SaveShadowPrefs()
        {
            var stringBuilder = new StringBuilder();
            foreach (var pref in _shadowPrefs)
            {
                stringBuilder.Append(pref.Key);
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(pref.Value);
                stringBuilder.Append(Environment.NewLine);
            }
            PlayerPrefs.SetString(_shadowPrefsKey, stringBuilder.ToString());
            Debug.Log($"Saved Prefs ({_shadowPrefs.Count})");
        }

        public static void LoadShadowPrefs()
        {
            var prefs = PlayerPrefs.GetString(_shadowPrefsKey).Split(Environment.NewLine);
            if (prefs.Length > 1)
            {
                for (int i = 0; i < prefs.Length - 1; i += 2)
                {
                    _shadowPrefs[prefs[i]] = prefs[i + 1];
                }
            }
            Debug.Log($"Loaded Prefs ({string.Join(",", _shadowPrefs)})");
        }
    }
}
