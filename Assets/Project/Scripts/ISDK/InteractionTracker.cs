// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Interactables only give access to Interactors via an Enumerator
    /// PointableElements give no access to Interactors
    /// This class lists both with lower overhead to get the count (no Linq)
    /// </summary>
    public class InteractionTracker : IDisposable
    {
        private IInteractableView _interactable;
        private IPointableElement _pointableElement;

        private List<IInteractorView> _interactors = new List<IInteractorView>();
        private List<IInteractorView> _selectingInteractors = new List<IInteractorView>();

        private int _timesHovered = 0;
        private int _timesSelected = 0;

        public int TimesHovered => _timesHovered;
        public int TimesSelected => _timesSelected;

        public IReadOnlyList<IInteractorView> Interactors => _interactors;
        public IReadOnlyList<IInteractorView> SelectingInteractors => _selectingInteractors;

        public event Action WhenChanged;
        public event Action<IInteractorView> WhenSelectAdded;
        public event Action<IInteractorView> WhenSelectRemoved;

        public object Subject => (_interactable as MonoBehaviour) ? _interactable : _pointableElement;

        public InteractionTracker(IInteractableView interactable)
        {
            _interactable = interactable;
            _interactable.WhenInteractorViewAdded += HandleHover;
            _interactable.WhenInteractorViewRemoved += HandleUnhover;
            _interactable.WhenSelectingInteractorViewAdded += HandleSelect;
            _interactable.WhenSelectingInteractorViewRemoved += HandleUnselect;
        }

        public InteractionTracker(IPointableElement pointableElement)
        {
            _pointableElement = pointableElement;
            _pointableElement.WhenPointerEventRaised += HandlePointerEvent;
        }

        public void Dispose()
        {
            if (_interactable != null)
            {
                _interactable.WhenInteractorViewAdded -= HandleHover;
                _interactable.WhenInteractorViewRemoved -= HandleUnhover;
                _interactable.WhenSelectingInteractorViewAdded -= HandleSelect;
                _interactable.WhenSelectingInteractorViewRemoved -= HandleUnselect;
                _interactable = null;
            }

            if (_pointableElement != null)
            {
                _pointableElement.WhenPointerEventRaised -= HandlePointerEvent;
                _pointableElement = null;
            }
        }

        public T GetSelectingInteractor<T>() where T : IInteractor
        {
            return TryGetSelectingInteractor<T>(out var result) ? result : throw new Exception($"{typeof(T)} not found");
        }

        public bool TryGetSelectingInteractor<T>(out T result) => TryFind(out result, _selectingInteractors);

        public T GetInteractor<T>() where T : IInteractor
        {
            return TryGetSelectingInteractor<T>(out var result) ? result : throw new Exception($"{typeof(T)} not found");
        }

        public bool TryGetInteractor<T>(out T result) where T : IInteractor => TryFind(out result, _interactors);

        private bool TryFind<T>(out T result, List<IInteractorView> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is T t)
                {
                    result = t;
                    return true;
                }
            }

            result = default;
            return false;
        }

        private void HandlePointerEvent(PointerEvent obj)
        {
            if (obj.Type == PointerEventType.Cancel)
            {
                HandleCancel(obj.Identifier);
                return;
            }

            if (!(obj.Data is IInteractorView interactor)) return;

            switch (obj.Type)
            {
                case PointerEventType.Hover:
                    HandleHover(interactor);
                    break;
                case PointerEventType.Unhover:
                    HandleUnhover(interactor);
                    break;
                case PointerEventType.Select:
                    HandleSelect(interactor);
                    break;
                case PointerEventType.Unselect:
                    HandleUnselect(interactor);
                    break;
                case PointerEventType.Move:
                    break;
                default:
                    throw new Exception($"Can't handle PointerEventType {obj.Type}");
            }
        }

        private void HandleCancel(int identifier)
        {
            int selectIndex = _selectingInteractors.FindIndex(x => x.Identifier == identifier);
            if (selectIndex >= 0) _selectingInteractors.RemoveAt(selectIndex);

            int index = _interactors.FindIndex(x => x.Identifier == identifier);
            if (index >= 0) _interactors.RemoveAt(index);

            WhenChanged?.Invoke();
        }

        private void HandleHover(IInteractorView obj)
        {
            _interactors.Add(obj);
            _timesHovered++;
            WhenChanged?.Invoke();
        }

        private void HandleUnhover(IInteractorView obj)
        {
            _interactors.Remove(obj);
            WhenChanged?.Invoke();
        }

        private void HandleSelect(IInteractorView obj)
        {
            _timesSelected++;

            _selectingInteractors.Add(obj);
            WhenSelectAdded?.Invoke(obj);
            WhenChanged?.Invoke();
        }

        private void HandleUnselect(IInteractorView obj)
        {
            _selectingInteractors.Remove(obj);
            WhenSelectRemoved?.Invoke(obj);
            WhenChanged?.Invoke();
        }

        public bool IsInteractor(IInteractorView obj) => _interactors.Contains(obj);

        public bool IsSelectingInteractor(IInteractorView obj) => _selectingInteractors.Contains(obj);

        /// <summary>
        /// Returns true if the interactable is selected by a 'Grab' type interactor (as opposed to Poke/Snap)
        /// </summary>
        public bool IsGrabbed()
        {
            return _selectingInteractors.FindIndex(IsGrabInteractor) != -1;
        }

        public bool IsSelected()
        {
            return _selectingInteractors.Count > 0;
        }

        public bool IsHovered()
        {
            return _interactors.Count > _selectingInteractors.Count;
        }

        public bool TryGetHand(out IHand hand)
        {
            return TryGetHand(_interactors, out hand);
        }

        public bool TryGetSelectingHand(out IHand hand)
        {
            return TryGetHand(_selectingInteractors, out hand);
        }

        private bool TryGetHand(List<IInteractorView> interactors, out IHand hand)
        {
            for (int i = 0; i < interactors.Count; i++)
            {
                if (TryGetHand(interactors[i], out hand))
                {
                    return true;
                }
            }

            hand = null;
            return false;
        }

        /// <summary>
        /// Utility method to check if an interactor is a grabber
        /// </summary>
        public static bool IsGrabInteractor(IInteractorView x)
        {
            if (x is DistanceHandGrabInteractor) return true; //<-- I missed this one
            if (x is TouchHandGrabInteractor) return true;
            if (x is DistanceGrabInteractor) return true;
            if (x is HandGrabInteractor) return true;
            if (x is GrabInteractor) return true;
            return false;
        }

        public bool TryGetHand(IInteractorView x, out IHand hand)
        {
            switch (x)
            {
                case DistanceHandGrabInteractor a: hand = a.Hand; return true;
                case HandGrabInteractor b: hand = b.Hand; return true;
            }

            if (x.Data is Component c)
            {
                var handComponent = c.GetComponentInParent(typeof(IHand));
                if (handComponent)
                {
                    hand = handComponent as IHand;
                    return true;
                }
            }

            hand = null;
            return false;
        }

        public override string ToString()
        {
            var targetName =
                _pointableElement is UnityEngine.Object o1 ? o1.name :
                _interactable is UnityEngine.Object o2 ? o2.name :
                "Unknown";

            return $"Target: {targetName}\n" +
                $"Select Count: {_selectingInteractors.Count}\n" +
                $"Hover Count: {_interactors.Count}";
        }

        public async Task ForceUnselectAll()
        {
            for (int i = SelectingInteractors.Count - 1; i >= 0; i--)
            {
                await ForceUnselect(i);
            }
        }

        public Task ForceUnselect(int selectingInteractorIndex)
        {
            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();

            // TODO better!
            switch (SelectingInteractors[selectingInteractorIndex])
            {
                case DistanceHandGrabInteractor x: InteractorExtensions<DistanceHandGrabInteractor, DistanceHandGrabInteractable>.ForceSelect(x, null, task); break;
                case TouchHandGrabInteractor x: InteractorExtensions<TouchHandGrabInteractor, TouchHandGrabInteractable>.ForceSelect(x, null, task); break;
                case DistanceGrabInteractor x: InteractorExtensions<DistanceGrabInteractor, DistanceGrabInteractable>.ForceSelect(x, null, task); break;
                case HandGrabInteractor x: InteractorExtensions<HandGrabInteractor, HandGrabInteractable>.ForceSelect(x, null, task); break;
                case GrabInteractor x: InteractorExtensions<GrabInteractor, GrabInteractable>.ForceSelect(x, null, task); break;
                case SnapInteractor x: InteractorExtensions<SnapInteractor, SnapInteractable>.ForceSelect(x, null, task); break;
                default: break;
            }

            return task.Task;
        }
    }
}
