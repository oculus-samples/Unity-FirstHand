// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEditor;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class LanguageMenuItem
    {
        private const string _menuPath = "Project/Language/";

        private const string _default = _menuPath + "Default";
        [MenuItem(_default)]
        static void SetDefault() => SetLanguage("en");
        [MenuItem(_default, true)]
        static bool CheckDefault() => CheckLanguage(_english, "en_GB");

        private const string _english = _menuPath + "English (GB)";
        [MenuItem(_english)]
        static void SetEnglish() => SetLanguage("en_GB");
        [MenuItem(_english, true)]
        static bool CheckEnglish() => CheckLanguage(_english, "en_GB");

        private const string _french = _menuPath + "French";
        [MenuItem(_french)]
        static void SetFrench() => SetLanguage("fr_FR");
        [MenuItem(_french, true)]
        static bool CheckFrench() => CheckLanguage(_french, "fr_FR");

        private const string _german = _menuPath + "German";
        [MenuItem(_german)]
        static void SetGerman() => SetLanguage("de_GR");
        [MenuItem(_german, true)]
        static bool CheckGerman() => CheckLanguage(_german, "de_GR");

        private const string _spanish = _menuPath + "Spanish";
        [MenuItem(_spanish)]
        static void SetSpanish() => SetLanguage("es_ES");
        [MenuItem(_spanish, true)]
        static bool CheckSpanish() => CheckLanguage(_spanish, "es_ES");

        private const string _spanishLA = _menuPath + "Spanish (Latin America)";
        [MenuItem(_spanishLA)]
        static void SetSpanishLA() => SetLanguage("es_LA");
        [MenuItem(_spanishLA, true)]
        static bool CheckSpanishLA() => CheckLanguage(_spanishLA, "es_LA");

        private const string _chinese = _menuPath + "Chinese";
        [MenuItem(_chinese)]
        static void SetChinese() => SetLanguage("zh_CN");
        [MenuItem(_chinese, true)]
        static bool CheckChinese() => CheckLanguage(_chinese, "zh_CN");

        private const string _korean = _menuPath + "Korean";
        [MenuItem(_korean)]
        static void SetKorean() => SetLanguage("ko_KR");
        [MenuItem(_korean, true)]
        static bool CheckKorean() => CheckLanguage(_korean, "ko_KR");

        private const string _czech = _menuPath + "Czech";
        [MenuItem(_czech)]
        static void SetCzech() => SetLanguage("cs_CZ");
        [MenuItem(_czech, true)]
        static bool CheckCzech() => CheckLanguage(_czech, "cs_CZ");

        private const string _danish = _menuPath + "Danish";
        [MenuItem(_danish)]
        static void SetDanish() => SetLanguage("da_DK");
        [MenuItem(_danish, true)]
        static bool CheckDanish() => CheckLanguage(_danish, "da_DK");

        private const string _greek = _menuPath + "Greek";
        [MenuItem(_greek)]
        static void SetGreek() => SetLanguage("el_GR");
        [MenuItem(_greek, true)]
        static bool CheckGreek() => CheckLanguage(_greek, "el_GR");

        private const string _finnish = _menuPath + "Finnish";
        [MenuItem(_finnish)]
        static void SetFinnish() => SetLanguage("fi_FI");
        [MenuItem(_finnish, true)]
        static bool CheckFinnish() => CheckLanguage(_finnish, "fi_FI");

        private const string _italian = _menuPath + "Italian";
        [MenuItem(_italian)]
        static void SetItalian() => SetLanguage("it_IT");
        [MenuItem(_italian, true)]
        static bool CHeckItalian() => CheckLanguage(_italian, "it_IT");

        private const string _norwegian = _menuPath + "Norwegian";
        [MenuItem(_norwegian)]
        static void SetNorwegian() => SetLanguage("nb_NO");
        [MenuItem(_norwegian, true)]
        static bool CheckNorwegian() => CheckLanguage(_norwegian, "nb_NO");

        private const string _dutch = _menuPath + "Dutch";
        [MenuItem(_dutch)]
        static void SetDutch() => SetLanguage("nl_NL");
        [MenuItem(_dutch, true)]
        static bool CheckDutch() => CheckLanguage(_dutch, "nl_NL");

        private const string _polish = _menuPath + "Polish";
        [MenuItem(_polish)]
        static void SetPolish() => SetLanguage("pl_PL");
        [MenuItem(_polish, true)]
        static bool CheckPolish() => CheckLanguage(_polish, "pl_PL");

        private const string _romanian = _menuPath + "Romanian";
        [MenuItem(_romanian)]
        static void SetRomanian() => SetLanguage("ro_RO");
        [MenuItem(_romanian, true)]
        static bool CheckRomanian() => CheckLanguage(_romanian, "ro_RO");

        private const string _swedish = _menuPath + "Swedish";
        [MenuItem(_swedish)]
        static void SetSwedish() => SetLanguage("sv_SV");
        [MenuItem(_swedish, true)]
        static bool CheckSwedish() => CheckLanguage(_swedish, "sv_SV");

        private const string _japanese = _menuPath + "Japanese";
        [MenuItem(_japanese)]
        static void SetJapanese() => SetLanguage("ja_JP");
        [MenuItem(_japanese, true)]
        static bool CheckJapanese() => CheckLanguage(_japanese, "ja_JP");

        private const string _portuguese = _menuPath + "Portuguese";
        [MenuItem(_portuguese)]
        static void SetPortuguese() => SetLanguage("pt_PT");
        [MenuItem(_portuguese, true)]
        static bool CheckPortuguese() => CheckLanguage(_portuguese, "pt_PT");

        private const string _portugueseBR = _menuPath + "Portuguese (Brazil)";
        [MenuItem(_portugueseBR)]
        static void SetPortugueseBrazil() => SetLanguage("pt_BR");
        [MenuItem(_portugueseBR, true)]
        static bool CheckPortugueseBrazil() => CheckLanguage(_portugueseBR, "pt_BR");

        private const string _russian = _menuPath + "Russian";
        [MenuItem(_russian)]
        static void SetRussian() => SetLanguage("ru_RU");
        [MenuItem(_russian, true)]
        static bool CheckRussian() => CheckLanguage(_russian, "ru_RU");

        private const string _turkish = _menuPath + "Turkish";
        [MenuItem(_turkish)]
        static void SetTurkish() => SetLanguage("tr_TR");
        [MenuItem(_turkish, true)]
        static bool CheckTurkish() => CheckLanguage(_turkish, "tr_TR");

        private static bool CheckLanguage(string path, string code)
        {
            Menu.SetChecked(path, PlayerPrefs.GetString("language", "en") == code);
            return true;
        }

        private static void SetLanguage(string code)
        {
            PlayerPrefs.SetString("language", code);
        }
    }
}
