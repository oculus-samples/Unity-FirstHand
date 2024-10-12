// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the gameplay of printing and assembling the glove
    /// </summary>
    public class GloveBuildBehaviour : MonoBehaviour
    {
        [SerializeField]
        private ProgressTracker _progress;
        [SerializeField]
        private int _beginProgress = 50;
        [SerializeField]
        private int _finishProgress = 60;
        [SerializeField]
        private int _endProgress = 100;

        [Space(8)]
        [SerializeField]
        PlayableDirector _mountTimeline;
        [SerializeField]
        private float _beginTime = 1;

        [Header("Assembling")]
        [SerializeField]
        private GloveGameObjects _schematicGrabbables;
        [SerializeField]
        private GloveGameObjects _schematicInteractables;
        [SerializeField]
        private GloveGameObjects _printedGrabbables;
        [SerializeField]
        private GloveGameObjects _gloveParts;
        [SerializeField]
        private GloveDropZones _printedDropZones;
        [SerializeField]
        private GloveGameObjects _dropZoneHighlights;
        [SerializeField]
        private GloveGameObjects _dropZoneUnhighlights;
        [SerializeField]
        private Transform _rotateAxis;
        [SerializeField]
        private AudioTrigger _rotateAudio;
        [SerializeField]
        private UICanvas _schematicSelectionCanvas;
        [SerializeField]
        private UICanvas _printButtonCanvas;
        [SerializeField]
        PlayableDirector _printingTimeline;

        [Header("Finishing")]
        [SerializeField]
        private float _crystalTime = 2.475f;
        [SerializeField]
        private float _crystalAngle;
        [SerializeField]
        private UICanvas _crystalCanvas;
        [SerializeField]
        SnapInteractable _crystalDropZone;
        [SerializeField]
        List<GameObject> _crystalObjects;
        [SerializeField]
        StringPropertyBehaviour _crystalColorChoice;
        [SerializeField]
        float _gloveTime = 2;
        [SerializeField]
        GloveBehaviour _leftGlove, _rightGlove;
        [SerializeField]
        private GameObject _crystalPrompt;
        [SerializeField]
        private UICanvas _progressBar;
        [SerializeField]
        private PlayableDirector _progressTimeline;
        [SerializeField]
        private PlayableDirector _progressTimeline2;

        [SerializeField, Optional]
        private ProgressTracker _partsProgress;

        private GlovePart _selectedSchematic;

        public GlovePart SelectedSchematic => _selectedSchematic;

        public bool IsSelectingSchematic { get; private set; }

        private void Start()
        {
            SetSelectedSchematic(GlovePart.None);

            ResetDirector(_mountTimeline);
            ResetDirector(_printingTimeline);
            ResetDirector(_progressTimeline);
            ResetDirector(_progressTimeline2);

            _gloveParts.SetAllActive(false);
            _printedGrabbables.SetAllActive(false);
            _dropZoneHighlights.SetAllActive(false);
            _schematicGrabbables.SetAllActive(false);
            _dropZoneUnhighlights.SetAllActive(false);

            _progressBar.Show(false);
            _crystalCanvas.Show(false);
            _printButtonCanvas.Show(false);
            _schematicSelectionCanvas.Show(false);

            _crystalDropZone.gameObject.SetActive(false);
            _crystalObjects.SetActive(false);
            _crystalPrompt.SetActive(false);

            if (_progress.Progress >= _endProgress)
            {
                _mountTimeline.Play();
                _gloveParts.SetAllActive(true);
                _leftGlove.SetWorn(true);
                _rightGlove.SetWorn(true);
            }
            else if (_progress.Progress >= _finishProgress)
            {
                Crystal();
            }
            else if (_progress.Progress >= _beginProgress)
            {
                Begin();
            }
        }

        /// <summary>
        /// Runs the initial sequence
        /// 1. put the hand mounts in place
        /// 2. show the dropzones
        /// 3. show the schematic selection ui
        /// </summary>
        public void Begin()
        {
            StartCoroutine(BeginRoutine());
            IEnumerator BeginRoutine()
            {
                yield return WaitForSecondsNonAlloc.WaitForSeconds(2f);
                _mountTimeline.Play();
                yield return new WaitForDirector(_mountTimeline, _beginTime);
                _mountTimeline.Pause();
                _dropZoneUnhighlights.SetAllActive(true);
                yield return WaitForSecondsNonAlloc.WaitForSeconds(1f);
                _schematicSelectionCanvas.Show(true);
            }
        }

        //for UnityEvent since UnityEvent cant serialize the enum
        public void SetSelectedSchematic(int glovePart) => SetSelectedSchematic((GlovePart)glovePart);

        public void SetSelectedSchematic(GlovePart glovePart)
        {
            _selectedSchematic = glovePart;
            _schematicGrabbables.SetActiveExclusive(_selectedSchematic);
            _printButtonCanvas.Show(glovePart != GlovePart.None);
            IsSelectingSchematic = glovePart != GlovePart.None;

            if (glovePart != GlovePart.None && _partsProgress.Progress <= 0)
            {
                _partsProgress.SetProgress(1);
            }
        }

        /// <summary>
        /// Runs the printing gameplay sequence
        /// 1. swap the schematic hologram for the 'printed' part
        /// 2. wait for the user to drop the part into the glove
        /// 3. show the UI for the next selection, or if all parts are in move to the finish sequence
        /// </summary>
        public void Print()
        {
            if (_selectedSchematic == GlovePart.None) return;

            StartCoroutine(PrintRoutine());
            IEnumerator PrintRoutine()
            {
                IsSelectingSchematic = false;

                // disable interaction on the hologram version
                _schematicInteractables[_selectedSchematic].SetActive(false);

                // hide the ui
                _schematicSelectionCanvas.Show(false);
                _printButtonCanvas.Show(false);

                SetDirectorTime(_progressTimeline, _progressTimeline.duration * _gloveParts.CountActive() / 4f);
                SetDirectorTime(_progressTimeline2, _progressTimeline2.duration * _gloveParts.CountActive() / 4f);
                _progressBar.Show(true);

                // turn on the 'printed' piece (expected to be in a disabled group)
                _printedGrabbables.SetActiveExclusive(_selectedSchematic);

                // play the print timeline and wait for it to finish
                _printingTimeline.Play();
                yield return new WaitForDirector(_printingTimeline);

                // wait for the user to drop the part into its dropzone
                var targetDropZone = _printedDropZones.GetSettings(_selectedSchematic);
                RotateGlove(targetDropZone.angle);

                _dropZoneHighlights.SetActiveExclusive(_selectedSchematic);
                yield return new WaitForDropZone(targetDropZone.zone);
                _dropZoneHighlights.SetAllActive(false);

                yield return new WaitForSeconds(0.3f);
                RotateGlove(0);

                // printed grabbable is the drop zone, turn it off the printed grabbable and the dropzone highlight
                _printedGrabbables.SetAllActive(false);
                _dropZoneUnhighlights[_selectedSchematic].SetActive(false);

                // turn on the noninteractive verison (inside the glove)
                _gloveParts[_selectedSchematic].SetActive(true);
                if (_partsProgress != null) _partsProgress.SetProgress(_gloveParts.CountActive() + 1);

                SetDirectorTime(_progressTimeline, _progressTimeline.duration * _gloveParts.CountActive() / 4f);
                SetDirectorTime(_progressTimeline2, _progressTimeline2.duration * _gloveParts.CountActive() / 4f);
                TweenRunner.DelayedCall(1f, () => _progressBar.Show(false));

                // clear the selection
                SetSelectedSchematic(GlovePart.None);
                ResetDirector(_printingTimeline);

                if (!_gloveParts.AllActive())
                {
                    _schematicSelectionCanvas.Show(true);
                }
                else
                {
                    _progress.SetProgress(_finishProgress);
                    Crystal();
                }
            }
        }

        /// <summary>
        /// Runs the finish assembly gameplay sequence
        /// 1. wait for the user to put a crystal in
        /// 2. create a copy for the left hand
        /// 3. animate the glove into the 'put on' position
        /// </summary>
        void Crystal()
        {
            StartCoroutine(CrystalRoutine());
            IEnumerator CrystalRoutine()
            {
                _printedGrabbables.SetAllActive(false);
                _dropZoneHighlights.SetAllActive(false);
                _schematicGrabbables.SetAllActive(false);
                _dropZoneUnhighlights.SetAllActive(false);

                _printButtonCanvas.Show(false);
                _schematicSelectionCanvas.Show(false);

                _gloveParts.SetAllActive(true);

                _mountTimeline.Resume();
                yield return new WaitForDirector(_mountTimeline, _crystalTime);
                _mountTimeline.Pause();

                RotateGlove(_crystalAngle);

                _crystalCanvas.Show(true);
                _crystalPrompt.SetActive(true);
                _crystalDropZone.gameObject.SetActive(true);
                yield return new WaitForDropZone(_crystalDropZone);
                _crystalPrompt.SetActive(false);
                _progress.Next();

                var held = _crystalDropZone.Interactors.First().GetComponentInParent<Grabbable>();
                var colorChoice = held.GetComponent<IProperty<string>>().Value;
                _crystalColorChoice.Value = colorChoice;

                held.gameObject.SetActive(false);
                _crystalDropZone.gameObject.SetActive(false);
                _crystalCanvas.Show(false);
                _crystalObjects.SetActive(true);

                RotateGlove(0);

                _mountTimeline.Resume();
                yield return new WaitForDirector(_mountTimeline, _gloveTime);
                _mountTimeline.Pause();

                _leftGlove.SetOpen(true);
                _rightGlove.SetOpen(true);

                yield return new WaitWhile(() => !(_leftGlove.IsWorn && _rightGlove.IsWorn));

                _mountTimeline.Resume();

                yield return new WaitForSeconds(2.77f);

                _progress.SetProgress(_endProgress);
            }
        }

        private void RotateGlove(float angle)
        {
            _rotateAudio.Play();
            TweenRunner.Kill(_rotateAxis);
            TweenRunner.Tween(_rotateAxis.localRotation, Quaternion.Euler(0, 0, angle), 0.6f, x => _rotateAxis.localRotation = x)
                .SetEase(Tween.Ease.QuadInOut)
                .SetID(_rotateAxis);
        }

        void ResetDirector(PlayableDirector d)
        {
            d.Stop();
            SetDirectorTime(d, 0);
        }

        private static void SetDirectorTime(PlayableDirector d, double dt)
        {
            d.time = dt;
            d.Evaluate();
        }

        public bool IsPartPrinted(GlovePart part)
        {
            return _gloveParts[part].activeSelf;
        }
    }

    [Serializable]
    struct GloveDropZones
    {
        public DropZoneSettings palm, knuckles, thumb, finger;

        public SnapInteractable Get(GlovePart part)
        {
            return GetSettings(part).zone;
        }

        public DropZoneSettings GetSettings(GlovePart part)
        {
            switch (part)
            {
                case GlovePart.Palm: return palm;
                case GlovePart.Thumb: return thumb;
                case GlovePart.Finger: return finger;
                case GlovePart.Knuckles: return knuckles;
                default: return default;
            }
        }

        [Serializable]
        public struct DropZoneSettings
        {
            public SnapInteractable zone;
            public float angle;
        }
    }

    [Serializable]
    struct GloveGameObjects
    {
        public GameObject palm, knuckles, thumb, finger;

        public GameObject this[int i] => Get((GlovePart)i);
        public GameObject this[GlovePart i] => Get(i);

        public GameObject Get(GlovePart part)
        {
            switch (part)
            {
                case GlovePart.Palm: return palm;
                case GlovePart.Thumb: return thumb;
                case GlovePart.Finger: return finger;
                case GlovePart.Knuckles: return knuckles;
                default: return null;
            }
        }

        public bool AllActive()
        {
            return palm.activeSelf && knuckles.activeSelf && thumb.activeSelf && finger.activeSelf;
        }

        public void SetActiveExclusive(GlovePart part) => ForEach((goPart, go) => go.SetActive(goPart == part));

        public void SetAllActive(bool active) => ForEach((_, go) => go.SetActive(active));

        private void ForEach(Action<GlovePart, GameObject> action)
        {
            for (int i = 1; i < (int)GlovePart._Count; i++)
            {
                action((GlovePart)i, this[i]);
            }
        }

        public int CountActive()
        {
            int result = 0;
            ForEach((_, go) => result += go.activeSelf ? 1 : 0);
            return result;
        }
    }

    public enum GlovePart
    {
        None,
        Palm,
        Knuckles,
        Thumb,
        Finger,
        [HideInInspector] _Count
    }
}
