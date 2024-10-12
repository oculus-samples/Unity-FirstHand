// Copyright (c) Meta Platforms, Inc. and affiliates.

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Maps TMPros preferred width onto its min width allowing ContentSizeFitters to use min width
    /// </summary>
    [ExecuteAlways]
    public class MinSizeIsPreferred : UIBehaviour, ILayoutElement
    {
        [SerializeField] private TextMeshProUGUI _text;

        protected override void OnEnable()
        {
            base.OnEnable();
            _text ??= GetComponent<TextMeshProUGUI>();
        }

        float ILayoutElement.minWidth => isActiveAndEnabled ? _text.preferredWidth : -1;
        float ILayoutElement.minHeight => isActiveAndEnabled ? _text.preferredHeight : -1;
        int ILayoutElement.layoutPriority => isActiveAndEnabled ? _text.layoutPriority : -1;

        float ILayoutElement.preferredWidth => -1;
        float ILayoutElement.flexibleWidth => -1;
        float ILayoutElement.preferredHeight => -1;
        float ILayoutElement.flexibleHeight => -1;

        void ILayoutElement.CalculateLayoutInputHorizontal() { }
        void ILayoutElement.CalculateLayoutInputVertical() { }
    }
}
