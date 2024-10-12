// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// A version of SecondaryInteractorFilter that references an IInteractableView instead of an IInteractable
    /// This should be fixed in an upcoming ISDK version
    /// </summary>
    public class SecondaryInteractorFilterFix : MonoBehaviour, IGameObjectFilter
    {
        [SerializeField, Interface(typeof(IInteractableView))]
        private UnityEngine.Object _primaryInteractable;
        public IInteractableView PrimaryInteractable { get; private set; }

        [SerializeField, Interface(typeof(IInteractable))]
        private UnityEngine.Object _secondaryInteractable;
        public IInteractable SecondaryInteractable { get; private set; }

        [SerializeField]
        private bool _selectRequired = false;

        private Dictionary<IInteractorView, List<IInteractorView>> _primaryToSecondaryMap;

        protected bool _started = false;

        protected virtual void Awake()
        {
            PrimaryInteractable = _primaryInteractable as IInteractableView;
            SecondaryInteractable = _secondaryInteractable as IInteractable;
            _primaryToSecondaryMap = new Dictionary<IInteractorView, List<IInteractorView>>();
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(PrimaryInteractable, nameof(PrimaryInteractable));
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                if (_selectRequired)
                {
                    PrimaryInteractable.WhenSelectingInteractorViewRemoved +=
                        HandleWhenSelectingInteractorViewRemoved;
                }
                else
                {
                    PrimaryInteractable.WhenInteractorViewRemoved +=
                        HandleWhenInteractorViewRemoved;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                if (_selectRequired)
                {
                    PrimaryInteractable.WhenSelectingInteractorViewRemoved -=
                        HandleWhenSelectingInteractorViewRemoved;
                }
                else
                {
                    PrimaryInteractable.WhenInteractorViewRemoved -=
                        HandleWhenInteractorViewRemoved;
                }
            }
        }

        public bool Filter(GameObject gameObject)
        {
            SecondaryInteractorConnection connection =
                gameObject.GetComponent<SecondaryInteractorConnection>();
            if (connection == null)
            {
                return false;
            }

            IEnumerable<IInteractorView> primaryViews = _selectRequired
                ? PrimaryInteractable.SelectingInteractorViews
                : PrimaryInteractable.InteractorViews;

            foreach (IInteractorView primaryView in primaryViews)
            {
                if (primaryView == connection.PrimaryInteractor)
                {
                    if (!_primaryToSecondaryMap.ContainsKey(primaryView))
                    {
                        _primaryToSecondaryMap.Add(primaryView, new List<IInteractorView>());
                    }

                    List<IInteractorView> secondaryViews = _primaryToSecondaryMap[primaryView];
                    if (!secondaryViews.Contains(connection.SecondaryInteractor))
                    {
                        secondaryViews.Add(connection.SecondaryInteractor);
                    }

                    return true;
                }
            }

            return false;
        }

        private void ClearInteractorsForPrimary(IInteractorView primary)
        {
            if (!_primaryToSecondaryMap.ContainsKey(primary))
            {
                return;
            }

            List<IInteractorView> secondaryViews = _primaryToSecondaryMap[primary];
            foreach (IInteractorView secondaryView in secondaryViews)
            {
                SecondaryInteractable.RemoveInteractorByIdentifier(secondaryView.Identifier);
            }

            _primaryToSecondaryMap.Remove(primary);
        }

        private void HandleWhenInteractorViewRemoved(IInteractorView primaryView)
        {
            ClearInteractorsForPrimary(primaryView);
        }

        private void HandleWhenSelectingInteractorViewRemoved(IInteractorView primaryView)
        {
            ClearInteractorsForPrimary(primaryView);
        }
    }
}
