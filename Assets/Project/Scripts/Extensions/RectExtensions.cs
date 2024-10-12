// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Utility functions for IMGUI Rects
    /// </summary>
    public static class RectExtensions
    {
        /// <summary>
        /// Returns a Rect inset from the paramater equally on all sides
        /// </summary>
        public static Rect Margin(this Rect rect, float all) => Margin(rect, all, all, all, all);

        /// <summary>
        /// Returns a Rect inset from the paramater equally on top/bottom and equally on left/right
        /// </summary>
        public static Rect Margin(this Rect rect, float leftRight, float topBottom) => Margin(rect, leftRight, leftRight, topBottom, topBottom);

        /// <summary>
        /// Returns a Rect inset from the paramater by the specified values
        /// </summary>
        public static Rect Margin(this Rect rect, float left, float right, float top, float bottom)
        {
            rect.height -= (top + bottom);
            rect.width -= (left + right);
            rect.x += left;
            rect.y += top;
            return rect;
        }

        /// <summary>
        /// Returns an array of rects representing the parameter divided vertially, with optional spacing between rects
        /// </summary>
        public static Rect[] DivideY(this Rect rect, int count, float spacing = 0)
        {
            var result = new Rect[count];
            result[0] = rect;
            float availableSpace = rect.height - spacing * (count - 1);
            result[0].height = availableSpace / count;
            for (int i = 1; i < count; i++)
            {
                result[i] = result[0];
                result[i].y += (result[0].height + spacing) * i;
            }
            return result;
        }

        /// <summary>
        /// Returns an array of rects representing the parameter divided horizontally, with optional spacing between rects
        /// </summary>
        public static Rect[] DivideX(this Rect rect, int count, float spacing = 0)
        {
            var result = new Rect[count];
            result[0] = rect;
            float availableSpace = rect.width - spacing * (count - 1);
            result[0].width = availableSpace / count;
            for (int i = 1; i < count; i++)
            {
                result[i] = result[0];
                result[i].y += (result[0].width + spacing) * i;
            }
            return result;
        }

        /// <summary>
        /// Splits the rect into two at the 'height' and returns the top half, out Rect remaining will contain the bottom half
        /// If height is negative it will be split relative to the bottom rather than the top
        /// </summary>
        public static Rect TakeTop(this Rect rect, float height, out Rect bottomHalf, float spacing = 0)
        {
            var topHalf = rect;
            bottomHalf = rect;
            topHalf.height = height < 0 ? topHalf.height + height : height; // using + subtract because height is negative
            bottomHalf.height = rect.height - topHalf.height - spacing;
            bottomHalf.y += topHalf.height + spacing;
            return topHalf;
        }

        /// <summary>
        /// Splits the rect into two at the 'height' and returns the bottom half, out Rect remaining will contain the top half
        /// If height is negative it will be split relative to the top rather than the bottom
        /// </summary>
        public static Rect TakeBottom(this Rect rect, float height, out Rect topHalf, float spacing = 0)
        {
            topHalf = TakeTop(rect, -height, out var bottomHalf);
            topHalf.height -= spacing;
            return bottomHalf;
        }

        /// <summary>
        /// Splits the rect into two at the 'width' and returns the left half, out Rect remaining will contain the right half
        /// If width is negative it will be split relative to the right edge rather than the left edge
        /// </summary>
        public static Rect TakeLeft(this Rect rect, float width, out Rect rightHalf, float spacing = 0)
        {
            var leftHalf = rect;
            rightHalf = rect;
            leftHalf.width = width < 0 ? leftHalf.width + width : width; // using + to subtract because width is negative
            rightHalf.width = rect.width - leftHalf.width - spacing;
            rightHalf.x += leftHalf.width + spacing;
            return leftHalf;
        }

        /// <summary>
        /// Splits the rect into two at the 'width' and returns the right half, out Rect remaining will contain the left half
        /// If width is negative it will be split relative to the left edge rather than the right edge
        /// </summary>
        public static Rect TakeRight(this Rect rect, float width, out Rect leftHalf, float spacing = 0)
        {
            leftHalf = TakeLeft(rect, -width, out var rightHalf);
            leftHalf.width -= spacing;
            return rightHalf;
        }
    }
}
