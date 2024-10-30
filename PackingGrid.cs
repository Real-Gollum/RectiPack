using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectiPack {

    /// <summary>
    /// Represents a rect/grid of rects that can be filled
    /// Basically an attempt on packing the rects
    /// </summary>
    internal class PackingGrid {

        /// <summary>
        /// Represents the size of the area currently encompassing all inserted rects
        /// </summary>
        public Rect currentSpanningRect {get; private set;}
        List<AreaRect> spaces;


        public PackingGrid(Rect rect) {
            Reset(rect);
        }


        public void Reset(Rect rect) {
            currentSpanningRect = new Rect();
            spaces = new List<AreaRect>() { new AreaRect() { x = 0, y = 0, width = rect.width, height = rect.height } };
        }


        public AreaRect? Insert(Rect rect) {
            
            // Iterate over all spaces in reverse order
            for (int i = spaces.Count - 1; i >= 0; i--) {

                AreaRect candidateSpace = spaces[i];

                SplitResultData result = Split.InsertAndSplit(rect, candidateSpace);

                if (result.result == SplitResult.failed) { continue; }
                
                // Debugging
                //if (candidateSpace.width < rect.width || candidateSpace.height < rect.height) { throw new Exception("Rect should not be able to fit"); }

                spaces.RemoveAt(i);

                if (result.newRects != null) {
                    for (int j = 0;  j < result.newRects.Length; j++) { spaces.Add(result.newRects[j]); }
                }

                AreaRect filledArea = new AreaRect() { x = candidateSpace.x, y = candidateSpace.y, width = rect.width, height = rect.height};
                currentSpanningRect = currentSpanningRect.ExpandWith(filledArea);


                //Console.WriteLine("Filled " + new Rect() { width = filledArea.width, height = filledArea.height } + " into " + new Rect() { width = candidateSpace.width, height = candidateSpace.height });

                return filledArea;
            }


            return null;
        }

        
        #if ENABLE_RECTIPACK_LOGGING
        public void PrintFreeSpaces() {
            Console.Write("Empty spaces: " + spaces.Count + "\t");
        }
        #endif

    }


}
