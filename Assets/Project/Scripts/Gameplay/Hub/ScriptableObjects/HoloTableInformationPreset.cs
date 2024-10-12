// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Scriptable object preset of HoloTable information
    /// </summary>
    [CreateAssetMenu(fileName = "HoloTableInformation",
        menuName = "ScriptableObjects/Hub/HoloTableInformation")]
    public class HoloTableInformationPreset : ScriptableObject
    {
        private static List<string> _keys = new List<string>();

        [Header("Scene ID")]
        [SerializeField]
        private string _id;
        public string Id => _id;

        [Header("Information")]
        [SerializeField]
        private string _locationName;
        public string LocationName => _locationName;

        [SerializeField]
        private Sprite _locationSprite, _inputTypeSprite;
        public Sprite LocationDisplaySprite => _locationSprite;
        public Sprite InputTypeSprite => _inputTypeSprite;

        [SerializeField]
        private string _inputHeader, _inputDescription;
        public string InputHeader => _inputHeader;
        public string InputDescription => _inputDescription;

        [SerializeField, Space(10)]
        private string _coinKeyPrefix;
        [SerializeField]
        private int _coinsTotalAmount;

        public int CoinsTotalAmount => _coinsTotalAmount;

        public int CoinsCollectedAmount
        {
            get
            {
                if (string.IsNullOrEmpty(_coinKeyPrefix)) return 0;

                // get all keys that match
                Store.GetAllKeys(_keys, x => x.StartsWith(_coinKeyPrefix));
                // count keys with value 'yes'
                int count = 0;
                _keys.ForEach(x => count += (Store.GetString(x) == "yes" ? 1 : 0));
                return count;
            }
        }
    }
}
