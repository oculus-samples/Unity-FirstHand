// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles displaying the holo table UI
    /// </summary>
    public class HoloTableUI : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private TextMeshProUGUI _locationName;
        [SerializeField]
        private UICanvas _dottedCircleInactive, _dottedCircleActive;
        [SerializeField]
        private StringPropertyBehaviour _hasLoaded;
        [SerializeField]
        private GameObject _linesParent;
        [SerializeField]
        private List<Image> _lines = new List<Image>();
        [SerializeField]
        private TextMeshProUGUI _coinText;

        [Header("Left Panels")]
        [SerializeField]
        private Graphic _locationImage;
        [SerializeField]
        private UICanvas _locationPanel, _directTouchPanel;
        [SerializeField]
        private UICanvas _voPanel, _voiceSDKPanel;
        [SerializeField]
        private TextMeshProUGUI _voiceOverDurationText;

        [Header("Right Panels")]
        [SerializeField]
        private Graphic _inputType;
        [SerializeField]
        private TextMeshProUGUI _inputHeaderText, _inputDescriptionText;
        [SerializeField]
        private UICanvas _inputTypePanel, _locomotionPanel;
        [SerializeField]
        private UICanvas _experienceModulePanel, _hapticsPanel;

        [Header("Additional")]
        [SerializeField]
        private Button _playButton, _replayButton;
        [SerializeField]
        private Button _skipButton, _replayVOButton;
        [SerializeField]
        private UICanvas _loadingCircleCanvas, _playCanvas, _skipIntroCanvas,
            _replayCanvas, _freeExploreCanvas;
        [SerializeField]
        private List<UICanvas> _voBars = new List<UICanvas>();
        [SerializeField]
        private List<ModuleInfo> _modules = new List<ModuleInfo>();

        private HoloTableInformationPreset _preset;

        [SerializeField]
        private List<HoloTableInformationPreset> _levelInfos = new List<HoloTableInformationPreset>();
        [SerializeField]
        StringPropertyRef _selection;
        [SerializeField]
        private List<LoaderDirectorInfo> _playableDirectors = new List<LoaderDirectorInfo>();
        [SerializeField]
        private List<LoaderDirectorInfo> _voiceOverPlayableDirectors = new List<LoaderDirectorInfo>();

        [SerializeField]
        AudioTrigger _loadedAudioTrigger;

        private const float _skipIntroDelayAmount = 0.5f;

        [SerializeField] ReferenceActiveState _canLoadLevelInfo;

        Coroutine _loadInfoRoutine;

        bool IsPresetValid => !string.IsNullOrWhiteSpace(_selection.Value);

        private void Start()
        {
            _playButton.onClick.AddListener(LoadLevel);
            _replayButton.onClick.AddListener(LoadLevel);
            _skipButton.onClick.AddListener(SkipVO);
            _replayVOButton.onClick.AddListener(ReplayVO);
            _selection.WhenChanged += UpdateUI;

            UpdateUI();
            UpdateCoinCounter();
            Store.WhenChanged += UpdateCoinCounter;
        }

        private void OnDestroy()
        {
            Store.WhenChanged -= UpdateCoinCounter;
        }

        private void UpdateCoinCounter()
        {
            var total = 0;
            var collected = 0;

            if (_preset.Id == "")
            {
                _levelInfos.ForEach(x => { total += x.CoinsTotalAmount; collected += x.CoinsCollectedAmount; });
            }
            else
            {
                total = _preset.CoinsTotalAmount;
                collected = _preset.CoinsCollectedAmount;
            }

            _coinText.SetText($"{collected}/<alpha=#60>{total}");
        }

        private void UpdateUI()
        {
            var preset = _levelInfos.Find(x => x.Id == _selection.Value);
            SetInformationPreset(preset);

            _hasLoaded.SetValue(preset.Id != "0" ? "yes" : "no");
        }

        public void SetInformationPreset(HoloTableInformationPreset preset)
        {
            if (_preset == preset) return;

            _preset = preset;

            _voiceOverPlayableDirectors.ForEach(x => x.director.Stop());

            if (_loadInfoRoutine != null)
            {
                StopCoroutine(_loadInfoRoutine);
                _loadInfoRoutine = null;
            }

            _loadInfoRoutine = StartCoroutine(LoadInformation());
            IEnumerator LoadInformation()
            {
                _loadingCircleCanvas.Show(true);

                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2f));
                yield return new WaitUntil(() => _canLoadLevelInfo);

                _loadingCircleCanvas.Show(false);

                if (IsPresetValid) _loadedAudioTrigger.Play();

                UpdateHoloTableInformation();
                UpdateCoinCounter();

                _loadInfoRoutine = null;
            }
        }

        private void UpdateHoloTableInformation()
        {
            if (!_preset) throw new System.Exception();

            _locationName.text = LocalizedText.GetUIText(_preset.LocationName);
            _locationImage.SetSprite(_preset.LocationDisplaySprite);
            _inputType.SetSprite(_preset.InputTypeSprite);
            _inputHeaderText.text = LocalizedText.GetUIText(_preset.InputHeader);
            _inputDescriptionText.text = LocalizedText.GetUIText(_preset.InputDescription);

            UpdatePanels();
        }

        private void Update()
        {
            if (IsPresetValid && !_loadingCircleCanvas.IsShown)
            {
                FillLines();
                LoaderDirectorInfo extraInfo = _voiceOverPlayableDirectors.Find(x => x.id == _preset.Id);
                var voDirector = extraInfo.director;
                var timerText = $"{voDirector.time.ToString("00:00")}/<alpha=#60>{voDirector.duration.ToString("00:00")}";
                _voiceOverDurationText.SetText(timerText);

                bool isVOPlaying = voDirector.state == PlayState.Playing;

                for (int i = 0; i < _voBars.Count; i++)
                    _voBars[i].Show(isVOPlaying);

                TweenRunner.DelayedCall(_skipIntroDelayAmount, () =>
                {
                    _skipIntroCanvas.Show(isVOPlaying);
                });

                var completed = extraInfo.complete.HasReference && extraInfo.complete;

                _playCanvas.Show(!isVOPlaying && !completed);
                _replayCanvas.Show(!isVOPlaying && completed);
                _replayVOButton.gameObject.SetActive(!isVOPlaying);
            }
            else
            {
                _skipIntroCanvas.Show(false);
                _playCanvas.Show(false);
                _replayCanvas.Show(false);
            }
        }

        private void UpdatePanels()
        {
            bool isPresetValid = IsPresetValid;
            PickActiveGameObject(_dottedCircleActive, _dottedCircleInactive, isPresetValid);
            PickActiveGameObject(_experienceModulePanel, _hapticsPanel, isPresetValid);
            PickActiveGameObject(_inputTypePanel, _locomotionPanel, isPresetValid);
            PickActiveGameObject(_locationPanel, _directTouchPanel, isPresetValid);
            PickActiveGameObject(_voPanel, _voiceSDKPanel, isPresetValid);
            EnableLines(isPresetValid);

            // set the right module image enabled
            _modules.ForEach(m => m.module.SetActive(m.id == _preset.Id));

            void PickActiveGameObject(UICanvas a, UICanvas b, bool pickA)
            {
                a.Show(pickA);
                b.Show(!pickA);
            }
        }

        private void EnableLines(bool isPresetValid)
        {
            _linesParent.SetActive(isPresetValid);

            for (int i = 0; i < _lines.Count; i++)
                _lines[i].fillAmount = 0;
        }

        private void FillLines()
        {
            if (PauseHandler.IsTimeStopped) return;

            float waitTime = 0.6f;
            for (int i = 0; i < _lines.Count; i++)
                _lines[i].fillAmount += 1f / waitTime * Time.deltaTime;
        }

        public void SkipVO()
        {
            LoaderDirectorInfo playableDirectorInfo = _voiceOverPlayableDirectors.Find(x => x.id == _preset.Id);
            playableDirectorInfo.director.time = playableDirectorInfo.director.duration - 0.02;
        }

        private void ReplayVO()
        {
            PlayableDirector director = _voiceOverPlayableDirectors.Find(x => x.id == _preset.Id).director;
            director.time = 0;
            director.Play();
        }

        private void LoadLevel()
        {
            _playableDirectors.Find(x => x.id == _preset.Id).director.Play();
        }

        [System.Serializable]
        private struct LoaderDirectorInfo
        {
            public string id;
            public PlayableDirector director;
            public ReferenceActiveState complete;
        }

        [System.Serializable]
        private struct ModuleInfo
        {
            public string id;
            public GameObject module;
        }
    }
}
