// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Auto sizes text groups
    /// </summary>
    public class TextAutoSizeGroup : MonoBehaviour
    {
        public List<TMP_Text> _texts;

        private void Reset()
        {
            GetComponentsInChildren(_texts);
        }

        private void OnEnable()
        {
            UpdateFontSizes();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(UpdateFontSizes);
        }

        private void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(UpdateFontSizes);
        }

        private void UpdateFontSizes(UnityEngine.Object t)
        {
            if (t == null || t is not TMP_Text text || !_texts.Contains(text)) return;

            UpdateFontSizes();
        }

        private void UpdateFontSizes()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(UpdateFontSizes);

            if (_texts == null || _texts.Count == 0)
                return;

            // Iterate over each of the text objects in the array to find a good test candidate
            // There are different ways to figure out the best candidate
            // Preferred width works fine for single line text objects
            int candidateIndex = 0;
            float maxPreferredWidth = 0;

            for (int i = 0; i < _texts.Count; i++)
            {
                float preferredWidth = _texts[i].preferredWidth;
                if (preferredWidth > maxPreferredWidth)
                {
                    maxPreferredWidth = preferredWidth;
                    candidateIndex = i;
                }
            }

            // Force an update of the candidate text object so we can retrieve its optimum point size.
            _texts[candidateIndex].enableAutoSizing = true;
            _texts[candidateIndex].ForceMeshUpdate();
            float optimumPointSize = _texts[candidateIndex].fontSize;

            // Disable auto size on our test candidate
            _texts[candidateIndex].enableAutoSizing = false;

            // Iterate over all other text objects to set the point size
            for (int i = 0; i < _texts.Count; i++)
            {
                _texts[i].enableAutoSizing = false;
                _texts[i].fontSize = optimumPointSize;
            }

            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(UpdateFontSizes);
        }
    }
}
