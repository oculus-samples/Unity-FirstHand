// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Localized text manager
    /// </summary>
    public class LocalizedText
    {
        private const string _default = "en";
        private static LocalizedTextList _localizedTextList, _localizedUIList;
        private static string _language;
        private static Regex _isTranslatable = new Regex("[a-zA-Z]+");
        private static Regex _removeRichText = new Regex("<[^>]+/?>");

        [RuntimeInitializeOnLoadMethod]
        private static void Start()
        {
            _language = GetLangagueCode();
            Debug.Log(_language);
            if (_language == _default) return;

            var subtitleText = Resources.Load<TextAsset>($"subtitles-{_language}");
            _localizedTextList = CreateFromJSON(subtitleText.text);

            var uiText = Resources.Load<TextAsset>($"ui-{_language}");
            _localizedUIList = CreateFromJSON(uiText.text);
        }

        private static string GetLangagueCode()
        {
            string code = PlayerPrefs.GetString("language", _default);
            if (code != _default) return code;

            // UnityEngine.SystemLanguage doesnt differentiate variations on English, Spanish and Portuguese
            // To differentiate these variations we pull from from android Locale
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    AndroidJavaObject locale = new AndroidJavaClass("java/util/Locale").CallStatic<AndroidJavaObject>("getDefault");
                    string language = locale.Call<string>("toString");
                    switch (language)
                    {
                        case "en_GB":
                        case "es_LA":
                        case "pt_BR":
                            return language;
                    }
                }
                catch { }
            }

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese: return "zh_TW";
                case SystemLanguage.Czech: return "cs_CZ";
                case SystemLanguage.Danish: return "da_DK";
                case SystemLanguage.Dutch: return "nl_NL";
                case SystemLanguage.Finnish: return "fi_FI";
                case SystemLanguage.French: return "fr_FR";
                case SystemLanguage.German: return "de_GR";
                case SystemLanguage.Greek: return "el_GR";
                case SystemLanguage.Italian: return "it_IT";
                case SystemLanguage.Japanese: return "ja_JP";
                case SystemLanguage.Korean: return "ko_KR";
                case SystemLanguage.Norwegian: return "nb_NO";
                case SystemLanguage.Polish: return "pl_PL";
                case SystemLanguage.Portuguese: return "pt_PT";
                case SystemLanguage.Romanian: return "ro_RO";
                case SystemLanguage.Russian: return "ru_RU";
                case SystemLanguage.Spanish: return "es_ES";
                case SystemLanguage.Swedish: return "sv_SE";
                case SystemLanguage.Turkish: return "tr_TR";
                case SystemLanguage.ChineseSimplified: return "zh_CN";
                case SystemLanguage.ChineseTraditional: return "zh_HK";
                default: return _default;
            }
        }

        public static string GetSubtitle(string id) => GetText(_localizedTextList, id);
        public static string GetUIText(string id) => GetText(_localizedUIList, id);

        private static string GetText(LocalizedTextList list, string id)
        {
            if (!Application.isPlaying) return id;
            if (_language == _default) return id;

            var id2 = _removeRichText.Replace(id, "");
            if (!_isTranslatable.IsMatch(id2)) return id;

            id2 = id2.Replace("\n", "").Replace("\r", "");

            if (list != null && list.TryGetValue(id2, out var value)) return value;

            string message = $"LOCALIZATION NOT FOUND: {_language}, {id}";

            LogError(list, id, message);

            return message;
        }

        private static void LogError(LocalizedTextList list, string id, string message)
        {
            UnityEngine.Object context = null;
            if (Application.isEditor && list == _localizedUIList)
            {
                context = Array.Find(Resources.FindObjectsOfTypeAll(typeof(TMP_Text)), x => (x as TMP_Text).text == id && !IgnoreLocalization.ShouldIgnore(x as TMP_Text));
            }

            Debug.LogError(message, context);
        }

        public static LocalizedTextList CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<LocalizedTextList>(jsonString);
        }
    }

    [Serializable]
    public class LocalizedTextList
    {
        private const CompareOptions _compare = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase;

        [SerializeField]
        private List<LocalizedTextElement> LocalizedTexts = new List<LocalizedTextElement>();

        public bool TryGetValue(string id, out string value)
        {
            for (int i = 0; i < LocalizedTexts.Count; i++)
            {
                var text = LocalizedTexts[i];
                if (IsMatch(text.ID, id) || IsMatch(text.AltID, id))
                {
                    value = text.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        bool IsMatch(string a, string b)
        {
            return string.Compare(a, b, CultureInfo.CurrentCulture, _compare) == 0;
        }
    }
}
