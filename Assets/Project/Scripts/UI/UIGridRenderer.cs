// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class UIGridRenderer : MaskableGraphic
    {
        static UIVertex[] _quad = new UIVertex[4] { UIVertex.simpleVert, UIVertex.simpleVert, UIVertex.simpleVert, UIVertex.simpleVert };

        [SerializeField, FormerlySerializedAs("m_GridColumns")]
        private int _columns = 10;
        [SerializeField, FormerlySerializedAs("m_GridRows")]
        private int _rows = 10;
        [SerializeField]
        private Vector2 _lineThickness;
        [SerializeField]
        private bool _borders = true;

        /// <summary>
        /// Number of columns in the Grid
        /// </summary>
        public int Columns
        {
            get => _columns;
            set
            {
                if (_columns == value) return;

                _columns = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Number of rows in the grid.
        /// </summary>
        public int Rows
        {
            get => _rows;
            set
            {
                if (_rows == value) return;

                _rows = value;
                SetVerticesDirty();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetVerticesDirty();
        }
#endif

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            for (int i = 0; i < _quad.Length; i++) _quad[i].color = color; // update the color for the grid

            Vector2 size = rectTransform.rect.size;
            Vector2 origin = -rectTransform.pivot * size;

            AddLines(_columns, 0);
            AddLines(_rows, 1);

            void AddLines(int count, int axis)
            {
                Vector2 start = origin;
                Vector2 end = origin + size; //start with a line diagonal across the rect

                for (int i = 0; i < count; i++)
                {
                    var t = i / (count - 1f); // normalize
                    if (!_borders && (t == 0 || t == 1)) continue; //skip borders if needed

                    start[axis] = end[axis] = origin[axis] + size[axis] * t; // set both positions along the axis to flatten it

                    AddLineSegment(vh, start, end);
                }
            }
        }

        private void AddLineSegment(VertexHelper vh, Vector2 start, Vector2 end)
        {
            Vector2 offset = new Vector2(start.y - end.y, end.x - start.x).normalized * _lineThickness / 2;

            _quad[0].position = _quad[0].uv0 = start - offset;
            _quad[1].position = _quad[1].uv0 = start + offset;
            _quad[2].position = _quad[2].uv0 = end + offset;
            _quad[3].position = _quad[3].uv0 = end - offset;

            vh.AddUIVertexQuad(_quad);
        }
    }
}
