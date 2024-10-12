// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Manages switching from the ISDK interactable instance of the drone, to the animated drone used in the minigame
    /// </summary>
    public class GrabbableDrone : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IInteractableView))]
        MonoBehaviour _interactableDrone;
        [SerializeField]
        DroneMiniGame _droneMiniGame;
        [SerializeField]
        GameObject _interactables;
        [SerializeField]
        GameObject _grabbableDroneVersion;
        [SerializeField]
        GameObject _gameDroneVersion;
        [SerializeField]
        ReferenceActiveState _allowStartWhen;
        [SerializeField]
        AudioTrigger _bootUp;

        int _selectingInteractors = 0;
        bool _wasPlaying = false;

        private void Start()
        {
            IInteractableView interactableView = _interactableDrone as IInteractableView;
            interactableView.WhenSelectingInteractorViewAdded += IncrementCounter;
            interactableView.WhenSelectingInteractorViewRemoved += DecrementCounter;

            _wasPlaying = _droneMiniGame.IsPlaying;
            _droneMiniGame.WhenChanged += TweenTransform;

            _gameDroneVersion.SetActive(false);
            _grabbableDroneVersion.SetActive(true);
        }

        private void TweenTransform()
        {
            if (!_wasPlaying && _droneMiniGame.IsPlaying)
            {
                _interactables.SetActive(false);
                _bootUp.Play();
                TweenRunner.TweenTransform(_grabbableDroneVersion.transform, _gameDroneVersion.transform, 2f)
                    .OnComplete(() =>
                    {
                        _gameDroneVersion.SetActive(true);
                        _grabbableDroneVersion.SetActive(false);
                    });
            }

            _wasPlaying = _droneMiniGame.IsPlaying;
        }

        private void DecrementCounter(IInteractorView obj)
        {
            if (_selectingInteractors > 0)
            {
                _selectingInteractors--;
                if (_selectingInteractors == 0 && _allowStartWhen)
                {
                    _droneMiniGame.StartGame();
                }
            }
        }

        private void IncrementCounter(IInteractorView obj)
        {
            _selectingInteractors++;
        }

        // called by message on Animator
        public void Done()
        {
            // workaround for offset bug when reanabling interactables
            Pose _gameDronePose = _gameDroneVersion.transform.GetPose();
            _gameDroneVersion.SetActive(false);
            var oldDrone = _grabbableDroneVersion;
            _grabbableDroneVersion = Instantiate(_grabbableDroneVersion, _gameDronePose.position, _gameDronePose.rotation, oldDrone.transform.parent);
            Destroy(oldDrone);

            _interactables = _grabbableDroneVersion.transform.Find("Interactables").gameObject;
            IInteractableView interactableView = _grabbableDroneVersion.GetComponent<IInteractableView>();
            interactableView.WhenSelectingInteractorViewAdded += IncrementCounter;
            interactableView.WhenSelectingInteractorViewRemoved += DecrementCounter;

            _grabbableDroneVersion.SetActive(true);
            _grabbableDroneVersion.transform.SetPose(_gameDronePose);
            _interactables.SetActive(true);
            TweenRunner.NextFrame(() => _grabbableDroneVersion.transform.SetPose(_gameDronePose)).SetUpdate(Tween.UpdateTime.LateUpdate);
        }
    }
}
