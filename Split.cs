using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace RectiPack {

    /// <summary>
    /// Represents the amount of splits created
    /// </summary>
    internal enum SplitResult {
        failed = -1,
        none,
        oneSplit,
        twoSplits
    }

    internal struct SplitResultData {
        public SplitResult result;

        /// <summary>
        /// Contains up to two new rects, the larger one always comes first.
        /// </summary>
        public AreaRect[] newRects;
    }
    
    internal static class Split {

        /// <summary>
        /// Tries to inser the given rect into the given area and creates new areas from the remaining space
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static SplitResultData InsertAndSplit(Rect rect, AreaRect area) {

            long freeSpaceW = (long)area.width - (long)rect.width;
            long freeSpaceH = (long)area.height - (long)rect.height;

            // Start by checking if the rectangle can even fit in the given area
            if (freeSpaceW < 0 || freeSpaceH < 0) { return new SplitResultData(){ result = SplitResult.failed }; }

            // Check if the rect fits exactly into the given area
            if (freeSpaceW == 0 && freeSpaceH == 0) { return new SplitResultData() { result = SplitResult.none }; }


            // Check if one dimension matches along both rects and only create one split
            if (freeSpaceW == 0) {
                return new SplitResultData() { result = SplitResult.oneSplit, newRects = [new AreaRect() { x = area.x, y = area.y + rect.height, width = area.width, height = area.height - rect.height }] };
            }

            if (freeSpaceH == 0) {
                return new SplitResultData() { result = SplitResult.oneSplit, newRects = [new AreaRect() { x = area.x + rect.width, y = area.y, width = area.width - rect.width, height = area.height }] };
            }


            // Otherwise the rect will fit into the area, but will leave some space along both dimensions.
            // Two splits are neccessary to get the remaining area.

            // Larger rectangles are preffered (makes insertion of future rectangles easier), so the split-axis has to be determined first:

            if (freeSpaceW > freeSpaceH) {
                // Split along the vertical axis

                return new SplitResultData() { result = SplitResult.twoSplits, newRects = [
                    new AreaRect() { x = area.x + rect.width, y = area.y, width = (uint)freeSpaceW, height = area.height },      // bigger rect
                    new AreaRect() { x = area.x, y = area.y + rect.height, width = rect.width, height = (uint)freeSpaceH },      // smaller rect
                ] };
            }

            // Otherwise split along the horizontal axis

            return new SplitResultData() { result = SplitResult.twoSplits,  newRects = [
                new AreaRect() { x = area.x, y = area.y + rect.height, width = area.width, height = (uint)freeSpaceH },      // bigger rect
                new AreaRect() { x = area.x + rect.width, y = area.y, width = (uint)freeSpaceW, height = rect.height },      // smaller rect
            ]};

        }


    }

}
