using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectiPack {

    /// <summary>
    /// Contains all implemented sorting comparers.
    /// Note: All rects should be sorted in decreasing order to achieve better results (largest to smallest area, etc.)
    /// </summary>
    public static class Comparators {
        /// <summary>
        /// Sorts the rectangles from largest area to smallest
        /// </summary>
        public static Comparer<Rect> OrderByArea = new OrderByArea();
        /// <summary>
        /// Sorts the rectangles from largest circumference to smallest
        /// </summary>
        public static Comparer<Rect> OrderByPerimeter = new OrderByPerimeter();
        /// <summary>
        /// Sorts the rectangles from largest side to smallest
        /// </summary>
        public static Comparer<Rect> OrderByBiggerSide = new OrderByBiggerSide();
        /// <summary>
        /// Sorts the rectangles from largest width to smallest
        /// </summary>
        public static Comparer<Rect> OrderByWidth = new OrderByWidth();
        /// <summary>
        /// Sorts the rectangles from largest height to smallest
        /// </summary>
        public static Comparer<Rect> OrderByHeight = new OrderByHeight();
    }



    #region Comparers

    internal class OrderByArea : Comparer<Rect> {
        public override int Compare(Rect left, Rect right) {
            return left.Area() > right.Area() ? -1 : 1;
        }
    }

    internal class OrderByPerimeter : Comparer<Rect> {
        public override int Compare(Rect left, Rect right) {
            return 2 * (left.width + left.height) > 2 * (right.width + right.height) ? -1 : 1;
        }
    }

    internal class OrderByBiggerSide : Comparer<Rect> {
        public override int Compare(Rect left, Rect right) {
            return Math.Max(left.width, left.height) > Math.Max(right.width, right.height) ? -1 : 1;
        }
    }

    internal class OrderByWidth : Comparer<Rect> {
        public override int Compare(Rect left, Rect right) {
            return left.width > right.width ? -1 : 1;
        }
    }

    internal class OrderByHeight : Comparer<Rect> {
        public override int Compare(Rect left, Rect right) {
            return left.height > right.height ? -1 : 1;
        }
    }

    #endregion
}
